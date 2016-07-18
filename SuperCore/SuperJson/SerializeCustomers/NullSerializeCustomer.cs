using System;
using Newtonsoft.Json.Linq;

namespace SuperJson.SerializeCustomers
{
    class NullSerializeCustomer : SerializeCustomer
    {
        public override bool UseCustomer(object obj, Type declaredType)
        {
            return obj == null;
        }

        public override JToken Serialize(object obj, SuperJsonSerializer serializer)
        {
            return new JValue((object)null);
        }
    }
}
