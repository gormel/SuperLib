using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperJson.Objects
{
    public abstract class SuperToken
    {
        public abstract SuperTokenType TokenType { get; }

        public object Value { get; set; }
    }
}
