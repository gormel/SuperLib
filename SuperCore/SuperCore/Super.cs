using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SuperCore
{
    public abstract class Super
    {
        private readonly Dictionary<string, object> mRegistred = new Dictionary<string, object>();

        private readonly AssemblyName mAssemblyName = new AssemblyName(Guid.NewGuid().ToString());

        private AssemblyBuilder mAssemblyBuilder;

        protected Super()
        {
            mAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(mAssemblyName, AssemblyBuilderAccess.RunAndSave);
        }

        protected CallResult Call(CallInfo info)
        {
            var obj = mRegistred[info.TypeName];
            var method = obj.GetType().GetMethod(info.MethodName);
            return new CallResult
            {
                CallID = info.CallID,
                Result = method.Invoke(obj, info.Args)
            };
        }

        internal abstract CallResult SendCall(CallInfo info);

        public T GetInstance<T>() where T : class
        {
            if (!typeof(T).IsInterface)
                throw new ArgumentException("T must be interface.");

            var moduleBuilder = mAssemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
            var typeBuilder = moduleBuilder.DefineType(
                typeof (T).FullName + "_" + Guid.NewGuid(), 
                TypeAttributes.Class | TypeAttributes.Public, 
                typeof(object), 
                new []{ typeof(T) });

            var fieldBuilder = typeBuilder.DefineField("mSuper", typeof (Super), FieldAttributes.Private);

            foreach (var method in typeof(T).GetMethods())
            {
                var methodBuilder = typeBuilder.DefineMethod(
                    method.Name,
                    method.Attributes,
                    method.ReturnType,
                    method.GetParameters().Select(i => i.ParameterType).ToArray());

                var generator = methodBuilder.GetILGenerator();
                
                //var info = new CallInfo();
                //info.TypeName = typeBuilder.Name;
                //info.MethodName = method.Name;
                //args = new object[method.GetParameters().Lenght];
                //for (int j = 0; j < args.Lenght; j++)
                //{
                //    args[j] = getArg(j);
                //}
                //info.Args = args;
                //return mSuper.SendCall(info);

                generator.DeclareLocal(typeof (object[]));//args
                generator.DeclareLocal(typeof (CallInfo));
                generator.Emit(OpCodes.Newobj, typeof(CallInfo));
                generator.Emit(OpCodes.Newobj, typeof(object[]));

            }

            var type = typeBuilder.CreateType();
            var result = Activator.CreateInstance(type);
            fieldBuilder.SetValue(result, this);

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

