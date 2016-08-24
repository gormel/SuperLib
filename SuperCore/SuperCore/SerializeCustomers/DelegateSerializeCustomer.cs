using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using SuperCore.Core;
using SuperCore.Wrappers;
using SuperJson;

namespace SuperCore.SerializeCustomers
{
	public class DelegateSerializeCustomer : SerializeCustomer
	{
		private SuperNet mSuper;

		public DelegateSerializeCustomer (SuperNet super)
		{
			mSuper = super;
		}
        
        public Type GetActionWrapper(Type[] argTypes)
        {
            if (argTypes.Length == 0)
                return typeof(DelegateActionWrapper);
            return typeof(DelegateActionWrapper<>).MakeGenericType(argTypes);
        }

        public Type GetFuncWrapper(Type resultType, Type[] argTypes)
        {
            if (argTypes.Length == 0)
                return typeof(DelegateFuncWrapper<>).MakeGenericType(resultType);
            return typeof(DelegateFuncWrapper<>).MakeGenericType(argTypes.Concat(new[] { resultType }).ToArray());
        }

        #region implemented abstract members of SerializeCustomer

        public override bool UseCustomer (object obj, Type declaredType)
		{
			return obj is Delegate;
		}

		public override JToken Serialize (object obj, Type declaredType, SuperJsonSerializer serializer)
		{
			var result = new JObject();

			var id = Guid.NewGuid();

			var typed = (Delegate)obj;

		    var delegateParameters = typed.Method.GetParameters().Select(p => p.ParameterType).ToArray();


            Type wrapperType = null;
		    if (typed.Method.ReturnType == typeof (void))
		        wrapperType = GetActionWrapper(delegateParameters);
		    else
		        wrapperType = GetFuncWrapper(typed.Method.ReturnType, delegateParameters);

			mSuper.Register(Activator.CreateInstance(wrapperType, typed), id);

			result.Add ("$type", "DelegateWrapper");
			result.Add ("ID", id.ToString ());
			result.Add ("DelegateType", typed.GetType ().AssemblyQualifiedName);

			var args = new JArray ();
			foreach (var param in typed.Method.GetParameters()) 
			{
				args.Add (param.ParameterType.AssemblyQualifiedName);
			}
			result.Add ("ArgumentTypes", args);
			result.Add ("ReturnType", typed.Method.ReturnType.AssemblyQualifiedName);

			return result;
		}

		#endregion
	}
}

