using System;
using System.Collections.Generic;
using SuperJson.Objects;

namespace SuperJson.SerializeCustomers
{
    class ArraySerializeCustomer : SerializeCustomer
    {
        public override bool UseCustomer(object obj, Type declaredType)
        {
            return obj.GetType().IsArray;
        }

        public override SuperToken Serialize(object obj, Type declaredType, SuperJsonSerializer serializer)
        {
            var arr = (Array)obj;
            var resultList = new List<SuperToken>();
            for (var i = 0; i < arr.Length; i++)
            {
                var elem = arr.GetValue(i);
                resultList.Add(serializer.Serialize(elem, obj.GetType().GetElementType()));
            }
            return new SuperArray(resultList.ToArray());
        }
    }
}
