using System;

namespace SuperCore
{
    public class CallInfo
    {
        public Guid CallID { get; } = Guid.NewGuid();

        public string TypeName;
        public string MethodName;
        public object[] Args;
    }
}
