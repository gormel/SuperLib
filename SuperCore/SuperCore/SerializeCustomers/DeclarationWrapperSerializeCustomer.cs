using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
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
