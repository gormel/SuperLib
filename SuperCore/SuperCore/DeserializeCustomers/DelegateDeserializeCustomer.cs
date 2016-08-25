using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using SuperCore.Core;
using SuperCore.Utilses;
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

		public override object Deserialize (JToken obj, SuperJsonSerializer serializer)
		{
			var delegateId = Guid.Parse (obj ["ID"].ToString ());
			var delegateType = Type.GetType (obj ["DelegateType"].ToString (), true);
			var argTypes = ((JArray)obj ["ArgumentTypes"])
				.Select (t => Type.GetType (t.ToString ())).ToArray ();
			var returnType = Type.GetType (obj ["ReturnType"].ToString ());

		    Type generatedInterfaceType = null;
		    if (returnType == typeof (void))
		        generatedInterfaceType = Utils.GetActionWrapper<IDelegateActionWrapperBase>(argTypes);
		    else
		        generatedInterfaceType = Utils.GetFuncWrapper<IDelegateFuncWrapperBase>(returnType, argTypes);

			//get interface instance from super using delegate id
			var inst = mSuper.GetInstance(generatedInterfaceType, delegateId);
			//create delegate from known method
			var result = Delegate.CreateDelegate(delegateType, inst, nameof(IDelegateActionWrapper.Invoke));
			return result;
		}

		#endregion
	}
}

