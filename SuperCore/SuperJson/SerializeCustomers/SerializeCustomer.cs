using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SuperJson
{
    public abstract class SerializeCustomer
    {
        public abstract bool UseCustomer(object obj, Type declaredType);

        public abstract JToken Serialize(object obj, Type declaredType, SuperJsonSerializer serializer);
    }
}
