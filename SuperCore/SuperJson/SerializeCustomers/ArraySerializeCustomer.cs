using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperJson.SerializeCustomers
{
    class ArraySerializeCustomer : SerializeCustomer
    {
        public override bool UseCustomer(object obj, Type declaredType)
        {
            return obj.GetType().IsArray;
        }

        public override string Serialize(object obj, SuperJsonSerializer serializer)
        {
            var arr = (Array)obj;
            var result = "[";
            for (int i = 0; i < arr.Length; i++)
            {
                var elem = arr.GetValue(i);
                result += serializer.Serialize(elem);
                if (i < arr.Length - 1)
                    result += ",";
            }
            result += "]";
            return result;
        }
    }
}
