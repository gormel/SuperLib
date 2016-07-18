using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SuperCore.NetData;
using SuperJson;

namespace SuperCore.Core
{
    public abstract class Super
    {
        private readonly Dictionary<string, object> mRegistred = new Dictionary<string, object>();

        private readonly Dictionary<Guid, object> mIdRegistred = new Dictionary<Guid, object>(); 

        private readonly AssemblyName mAssemblyName = new AssemblyName(Guid.NewGuid().ToString());

        private readonly AssemblyBuilder mAssemblyBuilder;

        protected Super()
        {
            mAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(mAssemblyName, AssemblyBuilderAccess.RunAndSave);
        }

        protected CallResult Call(CallInfo info)
        {
            var obj = info.ClassID == Guid.Empty ? mRegistred[info.TypeName] : mIdRegistred[info.ClassID];
            var method = obj.GetType().GetMethod(info.MethodName);

            var result = method.Invoke(obj, method.GetParameters()
                .Select((p, i) => SuperJsonSerializer.ConvertResult(info.Args[i], p.ParameterType)).ToArray());
            
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

        public T GetInstance<T>(Guid id = new Guid()) where T : class
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("T must be interface.");
            
            var moduleBuilder = mAssemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
            var typeBuilder = moduleBuilder.DefineType(
                typeof (T).Name + "_" + Guid.NewGuid(), 
                TypeAttributes.Class | TypeAttributes.Public, 
                typeof(object), 
                new []{ typeof(T) });

            var idFieldBuilder = typeBuilder.DefineField("ID", typeof (Guid), FieldAttributes.Public);

            var superFieldBuilder = typeBuilder.DefineField("mSuper", typeof (Super), FieldAttributes.Private);

            var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis,
                new[] { typeof (Super) });
            var ctorIL = ctor.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_1);
            ctorIL.Emit(OpCodes.Stfld, superFieldBuilder);
            ctorIL.Emit(OpCodes.Ret);

            foreach (var method in CollectMethods(typeof(T)))
            {
                var methodBuilder = typeBuilder.DefineMethod(
                    method.DeclaringType == typeof(T) ? method.Name : $"{method.DeclaringType.Name}.{method.Name}",
                    MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.NewSlot,
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

                generator.DeclareLocal(typeof (CallInfo));//loc_0
                generator.DeclareLocal(typeof (object[]));//loc_1
                generator.DeclareLocal(typeof (CallResult));//loc_2

                //var result(loc_0) = new CallInfo()
                generator.Emit(OpCodes.Newobj, typeof(CallInfo).GetConstructor(Type.EmptyTypes));
                generator.Emit(OpCodes.Stloc_0);

                //info.TypeName = $"{typeof(T).AssemblyQualifiedName}";
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldstr, typeof(T).AssemblyQualifiedName);
                generator.Emit(OpCodes.Stfld, typeof(CallInfo).GetField(nameof(CallInfo.TypeName)));

                //info.MethodName = $"{method.Name}";
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldstr, method.Name);
                generator.Emit(OpCodes.Stfld, typeof(CallInfo).GetField(nameof(CallInfo.MethodName)));

                //var args(loc_1) = new object[method.GetParameters().Length];
                generator.Emit(OpCodes.Ldc_I4, method.GetParameters().Length);
                generator.Emit(OpCodes.Newarr, typeof(object));
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
                    generator.Emit(OpCodes.Stelem, typeof(object));
                    k++;
                }

                //info.Args = args;
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldloc_1);
                generator.Emit(OpCodes.Stfld, typeof(CallInfo).GetField(nameof(CallInfo.Args)));
               
                //info.ClassID = this.ID;
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, idFieldBuilder);
                generator.Emit(OpCodes.Stfld, typeof(CallInfo).GetField(nameof(CallInfo.ClassID)));
               

                //var callResult(loc_2) = this.mSuper.SendCall(info);
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, superFieldBuilder);
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Callvirt, typeof(Super).GetMethod(nameof(Super.SendCall), BindingFlags.Instance | BindingFlags.Public));
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

            var type = typeBuilder.CreateType();
            var result = Activator.CreateInstance(type, this);
            type.GetField("ID").SetValue(result, id);

            return result as T;
        }

        public void Register<T>(T inst, Guid id = new Guid()) where T : class
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("T must be interface.");

            if (id != Guid.Empty)
            {
                mIdRegistred.Add(id, inst);
                return;
            }
            mRegistred.Add(typeof(T).AssemblyQualifiedName, inst);
        }
    }
}