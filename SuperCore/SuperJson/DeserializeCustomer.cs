using System;
using SuperJson.Objects;

namespace SuperJson
{
    public abstract class DeserializeCustomer
    {
        public abstract bool UseCustomer(SuperToken obj, Type declaredType);

        public abstract object Deserialize(SuperToken obj, SuperJsonSerializer serializer);
    }
}