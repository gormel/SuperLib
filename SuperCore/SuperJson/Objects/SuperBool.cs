using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperJson.Objects
{
    public class SuperBool : SuperToken
    {
        public SuperBool(bool value)
        {
            Value = value;
        }

        public override SuperTokenType TokenType => SuperTokenType.Bool;

        public bool TypedValue => (bool) Value;
    }
}
