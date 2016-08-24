using System;
using Newtonsoft.Json.Linq;
using SuperCore.Wrappers;
using SuperJson;

namespace SuperCore.SerializeCustomers
{
    internal class DeclarationWrapperSerializeCustomer : SerializeCustomer
    {
        public override bool UseCustomer(object obj, Type declaredType)
        {
            return obj is DeclarationWrapper;
        }

        public override JToken Serialize(object obj, Type declaredType, SuperJsonSerializer serializer)
        {
            var typed = (DeclarationWrapper)obj;
            return serializer.Serialize(typed.Instance, Type.GetType(typed.TypeName));
        }
    }
}
