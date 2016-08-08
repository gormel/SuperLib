using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SuperJson
{
    public abstract class DeserializeCustomer
    {
        public abstract bool UseCustomer(JToken obj, Type declaredType);

        public abstract object Deserialize(JToken obj, SuperJsonSerializer serializer);
    }
}
