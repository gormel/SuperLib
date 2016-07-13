using System;

namespace SuperJson.SerializeCustomers
{
    class StringSerializeCustomer : SerializeCustomer
    {
        public override bool UseCustomer(object obj, Type declaredType)
        {
            return obj is string;
        }

        public override string Serialize(object obj, SuperJsonSerializer serializer)
        {
            return "\"" + (string)obj + "\"";
        }
    }
}
