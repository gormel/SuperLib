using System;
using SuperJson.Objects;

namespace SuperJson.SerializeCustomers
{
    class StringSerializeCustomer : SerializeCustomer
    {
        public override bool UseCustomer(object obj, Type declaredType)
        {
            return obj is string;
        }

        public override SuperToken Serialize(object obj, Type declaredType, SuperJsonSerializer serializer)
        {
            return new SuperString((string)obj);
        }
    }
}
