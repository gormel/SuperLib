using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperJson.Objects
{
    public class SuperNull : SuperToken
    {
        public override SuperTokenType TokenType => SuperTokenType.Null;
    }
}
