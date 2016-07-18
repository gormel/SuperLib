using System;
using Newtonsoft.Json.Linq;

namespace SuperJson.SerializeCustomers
{
    class StringSerializeCustomer : SerializeCustomer
    {
        public override bool UseCustomer(object obj, Type declaredType)
        {
            return obj is string;
        }

        public override JToken Serialize(object obj, Type declaredType, SuperJsonSerializer serializer)
        {
            return new JValue((string)obj);
        }
    }
}
