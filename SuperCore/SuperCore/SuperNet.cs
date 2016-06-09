using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SuperCore
{
    public abstract class SuperNet : Super
    {
        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<CallResult>> mWaitingCalls 
            = new ConcurrentDictionary<Guid, TaskCompletionSource<CallResult>>(); 

        public override CallResult SendCall(CallInfo info)
        {
            var tcs = new TaskCompletionSource<CallResult>();
            mWaitingCalls.TryAdd(info.CallID, tcs);
            SendData(info);
            return tcs.Task.Result;
        }

        protected abstract void SendData(CallInfo info);

        protected void ReciveData(CallResult result)
        {
            TaskCompletionSource<CallResult> tcs;
            mWaitingCalls[result.CallID].SetResult(result);
            mWaitingCalls.TryRemove(result.CallID, out tcs);
        }
    }
}
