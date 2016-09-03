using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperJson.Objects
{
    public class SuperObject : SuperToken
    {
        public SuperObject()
        {
            Value = new Dictionary<string, SuperToken>();
        }

        public override SuperTokenType TokenType => SuperTokenType.Object;

        public Dictionary<string, SuperToken> TypedValue => (Dictionary<string, SuperToken>) Value;
    }
}
