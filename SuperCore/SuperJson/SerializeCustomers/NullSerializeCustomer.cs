using System;
using SuperJson.Objects;

namespace SuperJson.SerializeCustomers
{
    class NullSerializeCustomer : SerializeCustomer
    {
        public override bool UseCustomer(object obj, Type declaredType)
        {
            return obj == null;
        }

        public override SuperToken Serialize(object obj, Type declaredType, SuperJsonSerializer serializer)
        {
            return new SuperNull();
        }
    }
}
