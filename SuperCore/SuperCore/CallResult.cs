using System;

namespace SuperCore
{
    public enum TaskCompletionStatus
    {
        NoTask,
        NoTaskException,
        Canceled,
        Exception,
        Result,
    }
    public class CallResult
    {
        public Guid CallID { get; set; }

        public TaskCompletionStatus Status = TaskCompletionStatus.NoTask;

        public object Result;
    }
}
