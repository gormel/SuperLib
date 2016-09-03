using System;
using System.Reflection;
using SuperCore.Core;
using SuperJson;
using SuperJson.Objects;

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

        public override SuperToken Serialize(object obj, Type declaredType, SuperJsonSerializer serializer)
        {
            var result = new SuperObject { TypedValue = { { "$type", new SuperString("InterfaceWrapper") } }};

            var regType = declaredType;
            var regInst = obj;

            var registrationID = Guid.NewGuid();

            var registerMethod = typeof (Super).GetMethod(nameof(Super.Register),
                BindingFlags.Instance | BindingFlags.Public);
            registerMethod = registerMethod.MakeGenericMethod(regType);
            registerMethod.Invoke(mSuper, new[] { regInst, registrationID });

            result.TypedValue.Add("ID", new SuperString(registrationID.ToString()));
            result.TypedValue.Add("InterfaceType", new SuperString(regType.AssemblyQualifiedName));

            return result;
        }
    }
}
