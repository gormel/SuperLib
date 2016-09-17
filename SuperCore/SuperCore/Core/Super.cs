using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SuperCore.NetData;
using SuperJson;
using SuperCore.Wrappers;
using FieldAttributes = System.Reflection.FieldAttributes;
using MethodAttributes = System.Reflection.MethodAttributes;
using TypeAttributes = System.Reflection.TypeAttributes;
using System.Threading;
using SuperCore.Async.SyncContext;

namespace SuperCore.Core
{
    public abstract class Super
    {
        protected readonly ConcurrentDictionary<string, object> mRegistred = new ConcurrentDictionary<string, object>();

        protected readonly ConcurrentDictionary<Guid, object> mIdRegistred = new ConcurrentDictionary<Guid, object>(); 

        private readonly AssemblyName mAssemblyName = new AssemblyName(Guid.NewGuid().ToString());

        private readonly Dictionary<Type, Type> mGeneratedTypesCashe = new Dictionary<Type, Type>(); 

        internal readonly AssemblyBuilder mAssemblyBuilder;

        protected SuperSyncContext mContext;

        protected Super(SuperSyncContext context = null)
        {
            mContext = context ?? new SuperDefaultSyncContext();
            mAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(mAssemblyName, AssemblyBuilderAccess.RunAndSave);
        }

        internal CallResult Call(CallInfo info)
        {
            var obj = info.ClassID == Guid.Empty ? mRegistred[info.TypeName] : mIdRegistred[info.ClassID];
			var declaringType = obj?.GetType ();

			var method = declaringType.GetMethod(info.MethodName, 
				BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

            object result = null;
            var call = new Action(delegate 
                {
                    result = method.Invoke(obj, method.GetParameters()
                       .Select((p, i) => SuperJsonSerializer.ConvertResult(info.Args[i], p.ParameterType)).ToArray());
                });
            if (mContext != null)
            {
                mContext.Invoke(call);
            }
            else
            {
                call();
            }
            
            result = new DeclarationWrapper
            {
                Instance = result,
                TypeName = method.ReturnType.AssemblyQualifiedName
            };

            return new CallResult
            {
                CallID = info.CallID,
                Result = result
            };
        }

        public abstract CallResult SendCall(CallInfo info);
        public abstract void SendDestroy(string typeName, Guid classID);

        internal static List<MethodInfo> CollectMethods(Type interfaceType)
        {
            var result = new List<MethodInfo>();

            if (interfaceType == null)
                return result;

            foreach (var type in interfaceType.GetInterfaces())
            {
                result.AddRange(CollectMethods(type));
            }

            result.AddRange(CollectMethods(interfaceType.BaseType));

            result.AddRange(interfaceType.GetMethods());

            return result;
        }

        internal object GetInstance(Type interfaceType, Guid id)
        {

            if (!mGeneratedTypesCashe.ContainsKey(interfaceType))
            {
                var moduleBuilder = mAssemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
                var typeBuilder = moduleBuilder.DefineType(
                    interfaceType.Name + "_" + Guid.NewGuid(),
                    TypeAttributes.Class | TypeAttributes.Public,
                    typeof (object),
                    new[] {interfaceType});

                var idFieldBuilder = typeBuilder.DefineField("ID", typeof (Guid), FieldAttributes.Public);

                var superFieldBuilder = typeBuilder.DefineField("mSuper", typeof (Super), FieldAttributes.Private);

                var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis,
                    new[] {typeof (Super)});
                var ctorIL = ctor.GetILGenerator();
                ctorIL.Emit(OpCodes.Ldarg_0);
                ctorIL.Emit(OpCodes.Ldarg_1);
                ctorIL.Emit(OpCodes.Stfld, superFieldBuilder);
                ctorIL.Emit(OpCodes.Ret);

                foreach (var method in CollectMethods(interfaceType))
                {
                    var methodBuilder = typeBuilder.DefineMethod(
                        method.DeclaringType == interfaceType
                            ? method.Name
                            : $"{method.DeclaringType.Name}.{method.Name}",
                        MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig |
                        MethodAttributes.NewSlot,
                        CallingConventions.Standard | CallingConventions.HasThis,
                        method.ReturnType,
                        method.GetParameters().Select(i => i.ParameterType).ToArray());

                    var generator = methodBuilder.GetILGenerator();

                    //var info = new CallInfo();
                    //info.TypeName = typeof(T).AssemblyQualifiedName;
                    //info.MethodName = method.Name;
                    //args = new object[method.GetParameters().Lenght];
                    //args[0] = arg_1
                    //...
                    //args[n] = arg_n
                    //info.Args = args;
                    //info.ClassID = this.ID;
                    //return this.mSuper.SendCall(info);

                    generator.DeclareLocal(typeof (CallInfo)); //loc_0
                    generator.DeclareLocal(typeof (object[])); //loc_1
                    generator.DeclareLocal(typeof (CallResult)); //loc_2

                    //var result(loc_0) = new CallInfo()
                    generator.Emit(OpCodes.Newobj, typeof (CallInfo).GetConstructor(Type.EmptyTypes));
                    generator.Emit(OpCodes.Stloc_0);

                    //info.TypeName = $"{typeof(T).AssemblyQualifiedName}";
                    generator.Emit(OpCodes.Ldloc_0);
                    generator.Emit(OpCodes.Ldstr, interfaceType.AssemblyQualifiedName);
                    generator.Emit(OpCodes.Stfld, typeof (CallInfo).GetField(nameof(CallInfo.TypeName)));

                    //info.MethodName = $"{method.Name}";
                    generator.Emit(OpCodes.Ldloc_0);
                    generator.Emit(OpCodes.Ldstr, methodBuilder.Name);
                    generator.Emit(OpCodes.Stfld, typeof (CallInfo).GetField(nameof(CallInfo.MethodName)));

                    //var args(loc_1) = new object[method.GetParameters().Length];
                    generator.Emit(OpCodes.Ldc_I4, method.GetParameters().Length);
                    generator.Emit(OpCodes.Newarr, typeof (object));
                    generator.Emit(OpCodes.Stloc_1);

                    //args[0] = arg_1
                    //...
                    //args[n] = arg_n
                    int k = 1;
                    foreach (var parameter in method.GetParameters())
                    {
                        generator.Emit(OpCodes.Ldloc_1);
                        generator.Emit(OpCodes.Ldc_I4, k - 1);
                        generator.Emit(OpCodes.Ldarg_S, k);
                        if (parameter.ParameterType.IsValueType)
                        {
                            generator.Emit(OpCodes.Box, parameter.ParameterType);
                        }
                        generator.Emit(OpCodes.Stelem, typeof (object));
                        k++;
                    }

                    //info.Args = args;
                    generator.Emit(OpCodes.Ldloc_0);
                    generator.Emit(OpCodes.Ldloc_1);
                    generator.Emit(OpCodes.Stfld, typeof (CallInfo).GetField(nameof(CallInfo.Args)));

                    //info.ClassID = this.ID;
                    generator.Emit(OpCodes.Ldloc_0);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, idFieldBuilder);
                    generator.Emit(OpCodes.Stfld, typeof (CallInfo).GetField(nameof(CallInfo.ClassID)));


                    //var callResult(loc_2) = this.mSuper.SendCall(info);
                    generator.Emit(OpCodes.Ldarg_0);
                    generator.Emit(OpCodes.Ldfld, superFieldBuilder);
                    generator.Emit(OpCodes.Ldloc_0);
                    generator.Emit(OpCodes.Callvirt, typeof (Super).GetMethod(nameof(SendCall), BindingFlags.Instance | BindingFlags.Public));
                    generator.Emit(OpCodes.Stloc_2);

                    if (method.ReturnType != typeof (void))
                    {
                        //return callResult.Result;
                        generator.Emit(OpCodes.Ldloc_2);
                        generator.Emit(OpCodes.Ldfld, typeof (CallResult).GetField(nameof(CallResult.Result)));
                        if (method.ReturnType.IsValueType)
                            generator.Emit(OpCodes.Unbox_Any, method.ReturnType);
                    }

                    generator.Emit(OpCodes.Ret);
                    typeBuilder.DefineMethodOverride(methodBuilder, method);

                }

                //finalizer
                var finalizerBuilder = typeBuilder.DefineMethod("Finalize",
                    MethodAttributes.HideBySig | MethodAttributes.Private | MethodAttributes.NewSlot,
                    CallingConventions.HasThis | CallingConventions.Standard,
                    typeof(void),
                    new Type[0]);

                var finalizerGenerator = finalizerBuilder.GetILGenerator();

                //this.mSuper.SendDestroy(typeof(T).AssemblyQualifiedName, this.ID);
                
                finalizerGenerator.Emit(OpCodes.Ldarg_0);
                finalizerGenerator.Emit(OpCodes.Ldfld, superFieldBuilder);
                finalizerGenerator.Emit(OpCodes.Ldstr, interfaceType.AssemblyQualifiedName);
                finalizerGenerator.Emit(OpCodes.Ldarg_0);
                finalizerGenerator.Emit(OpCodes.Ldfld, idFieldBuilder);
                finalizerGenerator.Emit(OpCodes.Callvirt, typeof(Super).GetMethod(nameof(SendDestroy), BindingFlags.Instance | BindingFlags.Public));

                var type = typeBuilder.CreateType();
                mGeneratedTypesCashe.Add(interfaceType, type);
            }

            var cashedType = mGeneratedTypesCashe[interfaceType];

            var result = Activator.CreateInstance(cashedType, this);
            cashedType.GetField("ID").SetValue(result, id);

            return result;
        }

        public T GetInstance<T>(Guid id = new Guid()) where T : class
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("T must be interface.");

            return GetInstance(typeof(T), id) as T;
        }

		internal void Register(object inst, Guid id)
		{
			mIdRegistred.TryAdd(id, inst);
		}

        public void Register<T>(T inst, Guid id = new Guid()) where T : class
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("T must be interface.");

            if (id != Guid.Empty)
            {
				Register ((object)inst, id);
                return;
            }
            mRegistred.TryAdd(typeof(T).AssemblyQualifiedName, inst);
        }
    }
}