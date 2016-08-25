using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperCore.NetData
{
    internal class CallDestroyInfo : Call
    {
        public string TypeName { get; set; }
        public Guid ClassID { get; set; }
    }
}
