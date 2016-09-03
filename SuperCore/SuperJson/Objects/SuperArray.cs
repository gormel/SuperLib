using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperJson.Objects
{
    public class SuperArray : SuperToken
    {
        public SuperArray(SuperToken[] value)
        {
            Value = value;
        }
        public override SuperTokenType TokenType => SuperTokenType.Array;

        public SuperToken[] TypedValue => (SuperToken[]) Value;
    }
}
