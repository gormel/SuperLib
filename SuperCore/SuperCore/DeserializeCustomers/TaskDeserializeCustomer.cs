using System;
using System.Threading.Tasks;
using SuperCore.Core;
using SuperJson;
using SuperJson.Objects;

namespace SuperCore.DeserializeCustomers
{
    class TaskDeserializeCustomer : DeserializeCustomer
    {
        private readonly SuperNet mSuper;

        public TaskDeserializeCustomer(SuperNet super)
        {
            mSuper = super;
        }

        public override bool UseCustomer(SuperToken obj, Type declaredType)
        {
            if (!(obj is SuperObject))
                return false;
            return ((SuperObject)obj).TypedValue["$type"].Value.ToString() == "TaskWrapper";
        }

        public override object Deserialize(SuperToken obj, SuperJsonSerializer serializer)
        {
            var typed = (SuperObject) obj;
            var id = Guid.Parse(typed.TypedValue["ID"].Value.ToString());
            var resultType = Type.GetType(typed.TypedValue["ResultType"].Value.ToString());
            var tcsType = typeof(TaskCompletionSource<>).MakeGenericType(resultType);
            dynamic tcs = Activator.CreateInstance(tcsType);
            mSuper.WaitingTasks[id] = tcs;
            return tcs.Task;
        }
    }
}
