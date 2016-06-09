using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SuperCore
{
    public abstract class SuperNet : Super
    {
        private readonly JsonSerializer mSerializer = JsonSerializer.CreateDefault(new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        });

        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<CallResult>> mWaitingCalls 
            = new ConcurrentDictionary<Guid, TaskCompletionSource<CallResult>>(); 

        public override CallResult SendCall(CallInfo info)
        {
            var tcs = new TaskCompletionSource<CallResult>();
            mWaitingCalls.TryAdd(info.CallID, tcs);
            SendData(info);
            var result = tcs.Task.Result;
            var method = info.GetMethodInfo();
            if (method.ReturnType != result.Result.GetType())
            {
                result.Result = Convert.ChangeType(result.Result, method.ReturnType);
            }
            return result;
        }

        protected abstract void SendData(CallInfo info);

        protected void ReciveData(CallResult result)
        {
            TaskCompletionSource<CallResult> tcs;
            mWaitingCalls[result.CallID].SetResult(result);
            mWaitingCalls.TryRemove(result.CallID, out tcs);
        }

        protected async Task<object> GetObject(Socket socket)
        {
            var lenght = BitConverter.ToInt32(await socket.ReadBytes(4), 0);
            var packageData = Encoding.UTF8.GetString(await socket.ReadBytes(lenght));
            var result = mSerializer.Deserialize(new JsonTextReader(new StringReader(packageData)));
            return result;
        }

        protected byte[] GetBytes(object obj)
        {
            var stringBuilder = new StringBuilder();
            mSerializer.Serialize(new StringWriter(stringBuilder), obj);
            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }
    }
}
