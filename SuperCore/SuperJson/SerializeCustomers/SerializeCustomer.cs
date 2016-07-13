using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperJson
{
    public abstract class SerializeCustomer
    {
        public abstract bool UseCustomer(object obj, Type declaredType);

        public abstract string Serialize(object obj, SuperJsonSerializer serializer);
    }
}
