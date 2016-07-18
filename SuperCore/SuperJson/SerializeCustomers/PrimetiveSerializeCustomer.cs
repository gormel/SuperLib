using System;
using System.Globalization;
using Newtonsoft.Json.Linq;

namespace SuperJson.SerializeCustomers
{
    class PrimetiveSerializeCustomer : SerializeCustomer
    {
        public override bool UseCustomer(object obj, Type declaredType)
        {
            return obj.GetType().IsPrimitive;
        }

        public override JToken Serialize(object obj, Type declaredType, SuperJsonSerializer serializer)
        {
            return new JValue(obj);
        }
    }
}
