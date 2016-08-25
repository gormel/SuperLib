using System;

namespace SuperCore.NetData
{
    internal enum TaskCompletionStatus
    {
        Canceled,
        Exception,
        Result,
    }

    internal class TaskResult : Result
    {
        public Guid TaskID { get; set; }

        public TaskCompletionStatus Status { get; set; }

        public object Result;
    }
}
