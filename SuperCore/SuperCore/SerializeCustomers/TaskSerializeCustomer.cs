using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SuperCore.Core;
using SuperCore.NetData;
using SuperJson;

namespace SuperCore.SerializeCustomers
{
    class TaskSerializeCustomer : SerializeCustomer
    {
        private readonly SuperNet mSuper;

        public TaskSerializeCustomer(SuperNet super)
        {
            mSuper = super;
        }

        public override bool UseCustomer(object obj, Type declaredType)
        {
            return obj is Task;
        }

        public override JToken Serialize(object obj, SuperJsonSerializer serializer)
        {
            var typedValue = (Task)obj;

            var resultType = obj.GetType().GetGenericArguments()[0];
            var id = Guid.NewGuid();

            typedValue.ContinueWith(t =>
            {
                var taskResult = new TaskResult { TaskID = id };
                if (t.IsCanceled)
                {
                    taskResult.Status = TaskCompletionStatus.Canceled;
                    taskResult.Result = false;
                }
                else if (t.IsFaulted)
                {
                    taskResult.Status = TaskCompletionStatus.Exception;
                    taskResult.Result = t.Exception;
                }
                else
                {
                    var resultProp = t.GetType().GetProperty(nameof(Task<object>.Result));
                    taskResult.Status = TaskCompletionStatus.Result;
                    taskResult.Result = resultProp.GetValue(t);
                }

                mSuper.SendData(taskResult);
            });

            var result = new JObject
            {
                { "$type", "TaskWrapper" },
                { "ID", id.ToString() },
                { "ResultType", resultType.AssemblyQualifiedName }
            };
            
            return result;
        }
    }
}
