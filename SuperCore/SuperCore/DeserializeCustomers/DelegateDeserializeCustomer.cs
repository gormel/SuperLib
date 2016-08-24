using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using SuperCore.Core;
using SuperCore.Wrappers;
using SuperJson;

namespace SuperCore.DeserializeCustomers
{
	public class DelegateDeserializeCustomer : DeserializeCustomer
	{
		private SuperNet mSuper;
		public DelegateDeserializeCustomer (SuperNet super)
		{
			mSuper = super;
		}

		#region implemented abstract members of DeserializeCustomer

		public override bool UseCustomer (JToken obj, Type declaredType)
		{
			return obj["$type"]?.ToString() == "DelegateWrapper";
		}

	    public Type GetActionWrapper(Type[] argTypes)
	    {
	        if (argTypes.Length == 0)
	            return typeof (IDelegateActionWrapper);
	        return typeof(IDelegateActionWrapper<>).MakeGenericType(argTypes);
	    }

	    public Type GetFuncWrapper(Type resultType, Type[] argTypes)
	    {
	        if (argTypes.Length == 0)
	            return typeof (IDelegateFuncWrapper<>).MakeGenericType(resultType);
	        return typeof (IDelegateFuncWrapper<>).MakeGenericType(argTypes.Concat(new[] {resultType}).ToArray());
	    }

		public override object Deserialize (JToken obj, SuperJsonSerializer serializer)
		{
			var delegateId = Guid.Parse (obj ["ID"].ToString ());
			var delegateType = Type.GetType (obj ["DelegateType"].ToString (), true);
			var methodName = obj ["MethodName"].ToString ();
			var argTypes = ((JArray)obj ["ArgumentTypes"])
				.Select (t => Type.GetType (t.ToString ())).ToArray ();
			var returnType = Type.GetType (obj ["ReturnType"].ToString ());

		    Type generatedInterfaceType = null;
		    if (returnType == typeof (void))
		        generatedInterfaceType = GetActionWrapper(argTypes);
		    else
		        generatedInterfaceType = GetFuncWrapper(returnType, argTypes);

			//get interface instance from super using delegate id
			var inst = mSuper.GetInstance(generatedInterfaceType, delegateId);
			//create delegate from known method
			var result = Delegate.CreateDelegate(delegateType, inst, nameof(IDelegateActionWrapper.Invoke));
			return result;
		}

		#endregion
	}
}

