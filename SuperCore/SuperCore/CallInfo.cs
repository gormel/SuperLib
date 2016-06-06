using System;

namespace SuperCore
{
    public class CallInfo
    {
        public Guid CallID { get; } = Guid.NewGuid();

        public string TypeName { get; set; }
        public string MethodName { get; set; }
        public object[] Args { get; set; }
    }
}
