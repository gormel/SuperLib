using System;
using System.Linq;
using SuperCore.Core;
using SuperCore.Utilses;
using SuperCore.Wrappers;
using SuperJson;
using SuperJson.Objects;

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

		public override bool UseCustomer (SuperToken obj, Type declaredType)
		{
		    if (!(obj is SuperObject))
		        return false;
			return ((SuperObject)obj).TypedValue["$type"].Value.ToString() == "DelegateWrapper";
		}

		public override object Deserialize (SuperToken obj, SuperJsonSerializer serializer)
		{
		    var typedObj = (SuperObject) obj;

			var delegateId = Guid.Parse (typedObj.TypedValue["ID"].Value.ToString ());
			var delegateType = Type.GetType (typedObj.TypedValue["DelegateType"].Value.ToString (), true);
			var argTypes = ((SuperArray)typedObj.TypedValue["ArgumentTypes"])
				.TypedValue.Select (t => Type.GetType (t.Value.ToString ())).ToArray ();
			var returnType = Type.GetType (typedObj.TypedValue["ReturnType"].Value.ToString ());

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

