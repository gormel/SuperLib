using System;
using System.Reflection;
using SuperCore.Core;
using SuperJson;
using SuperJson.Objects;

namespace SuperCore.DeserializeCustomers
{
    internal class InterfaceDeserializeCustomer : DeserializeCustomer
    {
        private readonly Super mSuper;

        public InterfaceDeserializeCustomer(Super super)
        {
            mSuper = super;
        }

        public override bool UseCustomer(SuperToken obj, Type declaredType)
        {
            if (!(obj is SuperObject))
                return false;
            return ((SuperObject)obj).TypedValue["$type"].Value.ToString() == "InterfaceWrapper";
        }

        public override object Deserialize(SuperToken obj, SuperJsonSerializer serializer)
        {
            var typed = (SuperObject) obj;
            var instID = Guid.Parse(typed.TypedValue["ID"].Value.ToString());
            var interfaceType = Type.GetType(typed.TypedValue["InterfaceType"].Value.ToString());

            return mSuper.GetInstance(interfaceType, instID);
        }
    }
}
