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

        private readonly AssemblyName mAssemblyName = new AssemblyName(Guid.NewGuid().ToString());

        private readonly AssemblyBuilder mAssemblyBuilder;

        protected Super()
        {
            mAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(mAssemblyName, AssemblyBuilderAccess.RunAndSave);
        }

        protected CallResult Call(CallInfo info)
        {
            var obj = mRegistred[info.TypeName];
            var method = obj.GetType().GetMethod(info.MethodName);

            var result = method.Invoke(obj, method.GetParameters()
                .Select((p, i) => SuperJsonSerializer.ConvertResult(info.Args[i], p.ParameterType)).ToArray());
            return new CallResult
            {
                CallID = info.CallID,
                Result = result
            };
        }

        public abstract CallResult SendCall(CallInfo info);

        public T GetInstance<T>(Guid id = new Guid()) where T : class
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("T must be interface.");
            
            var moduleBuilder = mAssemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
            var typeBuilder = moduleBuilder.DefineType(
                typeof (T).FullName + "_" + Guid.NewGuid(), 
                TypeAttributes.Class | TypeAttributes.Public, 
                typeof(object), 
                new []{ typeof(T) });

            typeBuilder.DefineField("ID", typeof (Guid), FieldAttributes.Public);

            var superFieldBuilder = typeBuilder.DefineField("mSuper", typeof (Super), FieldAttributes.Private);

            var ctor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis,
                new[] { typeof (Super) });
            var ctorIL = ctor.GetILGenerator();
            ctorIL.Emit(OpCodes.Ldarg_0);
            ctorIL.Emit(OpCodes.Ldarg_1);
            ctorIL.Emit(OpCodes.Stfld, superFieldBuilder);
            ctorIL.Emit(OpCodes.Ret);

            foreach (var method in typeof(T).GetMethods())
            {
                var methodBuilder = typeBuilder.DefineMethod(
                    method.Name,
                    MethodAttributes.Public | MethodAttributes.Virtual,
                    method.ReturnType,
                    method.GetParameters().Select(i => i.ParameterType).ToArray());
                typeBuilder.DefineMethodOverride(methodBuilder, method);

                var generator = methodBuilder.GetILGenerator();

                //var info = new CallInfo();
                //info.TypeName = typeBuilder.Name;
                //info.MethodName = method.Name;
                //args = new object[method.GetParameters().Lenght];
                //args[0] = arg_1
                //...
                //args[n] = arg_n
                //info.Args = args;
                //return mSuper.SendCall(info);

                generator.DeclareLocal(typeof (CallInfo));//loc_0
                generator.DeclareLocal(typeof (object[]));//loc_1
                generator.DeclareLocal(typeof (CallResult));//loc_2

                //var result(loc_0) = new CallInfo()
                generator.Emit(OpCodes.Newobj, typeof(CallInfo).GetConstructor(Type.EmptyTypes));
                generator.Emit(OpCodes.Stloc_0);

                //result.TypeName = $"{typeBuilder.FullName}";
                generator.Emit(OpCodes.Ldloc_0);
                generator.Emit(OpCodes.Ldstr, typeof(T).FullName);
                generator.Emit(OpCodes.Stfld, typeof(CallInfo).GetField(nameof(CallInfo.TypeName)));

                //result.MethodName = $"{method.Name}";
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

                //var callResult(loc_2) = mSuper.SendCall(info);
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
                
            }

            var type = typeBuilder.CreateType();
            var result = Activator.CreateInstance(type, this);
            type.GetField("ID").SetValue(result, id);

            return result as T;
        }

        public void Register<T>(T inst) where T : class
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("T must be interface.");
            
            mRegistred.Add(typeof(T).FullName, inst);
        }
    }
}

