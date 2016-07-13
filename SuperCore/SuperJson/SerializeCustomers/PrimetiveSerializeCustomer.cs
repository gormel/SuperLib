using System;
using System.Globalization;

namespace SuperJson.SerializeCustomers
{
    class PrimetiveSerializeCustomer : SerializeCustomer
    {
        public override bool UseCustomer(object obj, Type declaredType)
        {
            return obj.GetType().IsPrimitive;
        }

        public override string Serialize(object obj, SuperJsonSerializer serializer)
        {
            if (obj is IFormattable)
                return ((IFormattable)obj).ToString(null, CultureInfo.InvariantCulture).ToLower();
            return obj.ToString().ToLower();
        }
    }
}
