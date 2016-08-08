using System;
using System.Reflection;
using Newtonsoft.Json.Linq;
using SuperCore.Core;
using SuperJson;

namespace SuperCore.SerializeCustomers
{
    class InterfaceSerializeCustomer : SerializeCustomer
    {
        private Super mSuper;

        public InterfaceSerializeCustomer(Super super)
        {
            mSuper = super;
        }

        public override bool UseCustomer(object obj, Type declaredType)
        {
            return declaredType?.IsInterface ?? false;
        }

        public override JToken Serialize(object obj, Type declaredType, SuperJsonSerializer serializer)
        {
            var result = new JObject { { "$type", "InterfaceWrapper" } };

            var regType = declaredType;
            var regInst = obj;

            var registrationID = Guid.NewGuid();

            var registerMethod = typeof (Super).GetMethod(nameof(Super.Register),
                BindingFlags.Instance | BindingFlags.Public);
            registerMethod = registerMethod.MakeGenericMethod(regType);
            registerMethod.Invoke(mSuper, new[] { regInst, registrationID });

            result.Add("ID", registrationID);
            result.Add("InterfaceType", regType.AssemblyQualifiedName);

            return result;
        }
    }
}
