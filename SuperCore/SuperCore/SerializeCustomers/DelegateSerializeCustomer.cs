using System;
using System.Collections.Generic;
using System.Linq;
using SuperCore.Core;
using SuperJson;
using SuperCore.Utilses;
using SuperCore.Wrappers;
using SuperJson.Objects;

namespace SuperCore.SerializeCustomers
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

		public override SuperToken Serialize (object obj, Type declaredType, SuperJsonSerializer serializer)
		{
			var result = new SuperObject();

			var id = Guid.NewGuid();

			var typed = (Delegate)obj;

		    var delegateParameters = typed.Method.GetParameters().Select(p => p.ParameterType).ToArray();


            Type wrapperType = null;
		    if (typed.Method.ReturnType == typeof (void))
		        wrapperType = Utils.GetActionWrapper<DelegateActionWrapperBase>(delegateParameters);
		    else
		        wrapperType = Utils.GetFuncWrapper<DelegateFuncWrapperBase>(typed.Method.ReturnType, delegateParameters);

			mSuper.Register(Activator.CreateInstance(wrapperType, typed), id);

			result.TypedValue.Add ("$type", new SuperString("DelegateWrapper"));
			result.TypedValue.Add ("ID", new SuperString(id.ToString ()));
			result.TypedValue.Add ("DelegateType", new SuperString(typed.GetType ().AssemblyQualifiedName));

		    var listArgs = new List<SuperToken>();
			foreach (var param in typed.Method.GetParameters()) 
			{
                listArgs.Add (new SuperString(param.ParameterType.AssemblyQualifiedName));
			}

			var args = new SuperArray(listArgs.ToArray());
			result.TypedValue.Add ("ArgumentTypes", args);
			result.TypedValue.Add ("ReturnType", new SuperString(typed.Method.ReturnType.AssemblyQualifiedName));

			return result;
		}

		#endregion
	}
}

