using System;
using System.Reflection;
using Newtonsoft.Json.Linq;
using SuperCore.Core;
using SuperJson;

namespace SuperCore.DeserializeCustomers
{
    internal class InterfaceDeserializeCustomer : DeserializeCustomer
    {
        private readonly Super mSuper;

        public InterfaceDeserializeCustomer(Super super)
        {
            mSuper = super;
        }

        public override bool UseCustomer(JToken obj, Type declaredType)
        {
            return obj["$type"]?.ToString() == "InterfaceWrapper";
        }

        public override object Deserialize(JToken obj, SuperJsonSerializer serializer)
        {
            var instID = Guid.Parse(obj["ID"].ToString());
            var interfaceType = Type.GetType(obj["InterfaceType"].ToString());

            var getInstanceMethod =
                typeof (Super).GetMethod(nameof(Super.GetInstance), BindingFlags.Instance | BindingFlags.Public)
                    .MakeGenericMethod(interfaceType);

            return getInstanceMethod.Invoke(mSuper, new object[] { instID });
        }
    }
}
