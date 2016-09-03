using System;
using SuperJson.Objects;

namespace SuperJson.SerializeCustomers
{
    class PrimetiveSerializeCustomer : SerializeCustomer
    {
        public override bool UseCustomer(object obj, Type declaredType)
        {
            return obj.GetType().IsPrimitive;
        }

        public override SuperToken Serialize(object obj, Type declaredType, SuperJsonSerializer serializer)
        {
            if (obj is bool)
                return new SuperBool((bool)obj);
            return new SuperNumber((double)SuperJsonSerializer.ConvertResult(obj, typeof(double)));
        }
    }
}
