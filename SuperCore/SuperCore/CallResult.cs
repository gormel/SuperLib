using System;

namespace SuperCore
{
    public class CallResult : Result
    {
        public Guid CallID { get; set; }

        public bool Exception { get; set; } = false;
        
        public object Result;
    }
}
