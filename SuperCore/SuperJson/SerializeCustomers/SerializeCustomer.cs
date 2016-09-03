using System;
using SuperJson.Objects;

namespace SuperJson
{
    public abstract class SerializeCustomer
    {
        public abstract bool UseCustomer(object obj, Type declaredType);

        public abstract SuperToken Serialize(object obj, Type declaredType, SuperJsonSerializer serializer);
    }
}
