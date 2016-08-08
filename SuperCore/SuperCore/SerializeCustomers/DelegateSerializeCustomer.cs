using System;
using SuperJson;
using SuperCore.Core;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace SuperCore
{
	public class DelegateSerializeCustomer : SerializeCustomer
	{
		private SuperNet mSuper;

		public DelegateSerializeCustomer (SuperNet super)
		{
			mSuper = super;
		}

		#region implemented abstract members of SerializeCustomer

		public override bool UseCustomer (object obj, Type declaredType)
		{
			return obj is Delegate;
		}

		public override JToken Serialize (object obj, Type declaredType, SuperJsonSerializer serializer)
		{
			var result = new JObject ();

			var id = Guid.NewGuid ();

			var typed = obj as Delegate;

			mSuper.Register (typed.Target ?? new StaticTypeInfoWrapper
				{
					TypeName = typed.Method.DeclaringType.AssemblyQualifiedName
				}, id);

			result.Add ("$type", "DelegateWrapper");
			result.Add ("ID", id.ToString ());
			result.Add ("DelegateType", typed.GetType ().AssemblyQualifiedName);
			result.Add ("MethodName", typed.Method.Name);

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

