using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SuperCore.Core;
using SuperJson;

namespace SuperCore.DeserializeCustomers
{
    class TaskDeserializeCustomer : DeserializeCustomer
    {
        private readonly SuperNet mSuper;

        public TaskDeserializeCustomer(SuperNet super)
        {
            mSuper = super;
        }

        public override bool UseCustomer(JToken obj, Type declaredType)
        {
            return obj["$type"]?.ToString() == "TaskWrapper";
        }

        public override object Deserialize(JToken obj, SuperJsonSerializer serializer)
        {
            var id = Guid.Parse(obj["ID"].ToString());
            var resultType = Type.GetType(obj["ResultType"].ToString());
            var tcsType = typeof(TaskCompletionSource<>).MakeGenericType(resultType);
            dynamic tcs = Activator.CreateInstance(tcsType);
            mSuper.WaitingTasks[id] = tcs;
            return tcs.Task;
        }
    }
}
