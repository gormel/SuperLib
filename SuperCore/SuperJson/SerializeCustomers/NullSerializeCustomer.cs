using System;

namespace SuperJson.SerializeCustomers
{
    class NullSerializeCustomer : SerializeCustomer
    {
        public override bool UseCustomer(object obj, Type declaredType)
        {
            return obj == null;
        }

        public override string Serialize(object obj, SuperJsonSerializer serializer)
        {
            return "null";
        }
    }
}
