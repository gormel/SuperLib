using System;

namespace SuperCore
{
    public enum TaskCompletionStatus
    {
        Canceled,
        Exception,
        Result,
    }

    public class TaskResult : Result
    {
        public Guid TaskID { get; set; }

        public TaskCompletionStatus Status { get; set; }

        public object Result;
    }
}
