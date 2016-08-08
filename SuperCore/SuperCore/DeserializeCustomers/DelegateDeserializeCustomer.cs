using System;
using SuperCore.Core;
using SuperJson;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Linq;

namespace SuperCore
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
			var gelegateId = Guid.Parse (obj ["ID"].ToString ());
			var delegateType = Type.GetType (obj ["DelegateType"].ToString ());
			var methodName = obj ["MethodName"].ToString ();
			var argTypes = ((JArray)obj ["ArgumentTypes"])
				.Select (t => Type.GetType (t.ToString ())).ToArray ();
			var returnType = Type.GetType (obj ["ReturnType"].ToString ());

			//generate interface
			var moduleBuilder = mSuper.mAssemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
			var typeBuilder = moduleBuilder.DefineType ($"<IDelegateWrapper>_{Guid.NewGuid()}", 
				TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Public);
			typeBuilder.DefineMethod (methodName,
				MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.Public,
				returnType,
				argTypes);
			var generatedInterfaceType = typeBuilder.CreateType ();
			//get interface instance from super using delegate id
			var inst = mSuper.GetType().GetMethod(nameof(Super.GetInstance))
				.MakeGenericMethod(generatedInterfaceType)
				.Invoke(mSuper, new object[] { gelegateId });
			//create delegate from known method
			var result = Delegate.CreateDelegate(delegateType, inst, methodName);
			return result;
		}

		#endregion
	}
}

