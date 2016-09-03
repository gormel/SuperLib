using System;
using SuperCore.Wrappers;
using SuperJson;
using SuperJson.Objects;

namespace SuperCore.SerializeCustomers
{
    internal class DeclarationWrapperSerializeCustomer : SerializeCustomer
    {
        public override bool UseCustomer(object obj, Type declaredType)
        {
            return obj is DeclarationWrapper;
        }

        public override SuperToken Serialize(object obj, Type declaredType, SuperJsonSerializer serializer)
        {
            var typed = (DeclarationWrapper)obj;
            return serializer.Serialize(typed.Instance, Type.GetType(typed.TypeName));
        }
    }
}
