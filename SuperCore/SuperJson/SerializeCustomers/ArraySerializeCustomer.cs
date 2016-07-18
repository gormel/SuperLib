using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SuperJson.SerializeCustomers
{
    class ArraySerializeCustomer : SerializeCustomer
    {
        public override bool UseCustomer(object obj, Type declaredType)
        {
            return obj.GetType().IsArray;
        }

        public override JToken Serialize(object obj, SuperJsonSerializer serializer)
        {
            var arr = (Array)obj;
            var result = new JArray();
            for (var i = 0; i < arr.Length; i++)
            {
                var elem = arr.GetValue(i);
                result.Add(serializer.Serialize(elem, obj.GetType().GetElementType()));
            }
            return result;
        }
    }
}
