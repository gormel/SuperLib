using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SuperCore.DeserializeCustomers;
using SuperCore.NetData;
using SuperCore.SerializeCustomers;
using SuperJson;
using System.Diagnostics;
using System.Threading;

namespace SuperCore.Core
{
    public abstract class SuperNet : Super
    {
        private readonly SuperJsonSerializer mSerializer = new SuperJsonSerializer();

        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<CallResult>> mWaitingCalls 
            = new ConcurrentDictionary<Guid, TaskCompletionSource<CallResult>>();

        //Guid => TaskCompletitionSource<?>
        internal readonly ConcurrentDictionary<Guid, dynamic> WaitingTasks 
            = new ConcurrentDictionary<Guid, dynamic>();

        protected SuperNet()
        {
            mSerializer.SerializeCustomers.Add(new TaskSerializeCustomer(this));
            mSerializer.DeserializeCustomers.Add(new TaskDeserializeCustomer(this));

            mSerializer.SerializeCustomers.Add(new InterfaceSerializeCustomer(this));
            mSerializer.DeserializeCustomers.Add(new InterfaceDeserializeCustomer(this));

			mSerializer.SerializeCustomers.Add (new DelegateSerializeCustomer (this));
			mSerializer.DeserializeCustomers.Add (new DelegateDeserializeCustomer (this));

            mSerializer.SerializeCustomers.Add(new DeclarationWrapperSerializeCustomer());
        }

        public override CallResult SendCall(CallInfo info)
        {
            var tcs = new TaskCompletionSource<CallResult>();
            mWaitingCalls.TryAdd(info.CallID, tcs);
            SendData(info);
            var method = info.GetMethodInfo();
            var result = tcs.Task.Result;
            result.Result = SuperJsonSerializer.ConvertResult(result.Result, method.ReturnType);
            return result;
        }

        protected async Task ReadClient(Socket client, CancellationToken stop = default(CancellationToken))
        {
            try
            {
                while (true)
                {
                    stop.ThrowIfCancellationRequested();
                    var resultObj = await GetObject(client);
                    stop.ThrowIfCancellationRequested();
                    var processedResult = ProcessResult(client, resultObj);
                }
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                ClientDisconnected(client);
            }
        }

        private async Task ProcessResult(Socket client, object resultObj)
        {
            if (resultObj is Call)
            {
                Result result;
                var send = ReciveCall((Call)resultObj, out result);
                if (!send)
                    return;

                var data = GetBytes(result);
                await client.SendBytes(BitConverter.GetBytes(data.Length));
                await client.SendBytes(data);
            }
            else if (resultObj is Result)
            {
                ReciveData((Result)resultObj);
            }
        }

        internal abstract void SendData(object info);

        protected abstract void ClientDisconnected(Socket client);

		protected bool ReciveCall(Call info, out Result result)
		{
			dynamic call = info;
			return ReciveCall(call, out result);
		}

		protected bool ReciveCall(CallInfo info, out Result result)
		{
			result = null;
			if (info.ClassID != Guid.Empty && !mIdRegistred.ContainsKey (info.ClassID))
				return false;
			if (info.ClassID == Guid.Empty && !mRegistred.ContainsKey (info.TypeName))
				return false;
			try
			{
				result = Call(info);
			}
			catch (Exception e)
			{
				result = new CallResult
				{
					CallID = (info).CallID,
					Result = e,
					Exception = true
				};
			}
			return true;
		}

        protected void ReciveData(Result result)
        {
            dynamic res = result;
            ReciveData(res);
        }

        protected void ReciveData(CallResult result)
        {
            TaskCompletionSource<CallResult> tcs;
			if (!mWaitingCalls.TryRemove (result.CallID, out tcs))
				return;
            if (result.Exception)
            {
                tcs.SetException((Exception)result.Result);
            }
            else
            {
                tcs.SetResult(result);
            }
        }

        protected void ReciveData(TaskResult result)
        {
            var tcs = WaitingTasks[result.TaskID];
            switch (result.Status)
            {
                case TaskCompletionStatus.Canceled:
                    tcs.SetCanceled();
                    break;
                case TaskCompletionStatus.Exception:
                    tcs.SetException((Exception)result.Result);
                    break;
                case TaskCompletionStatus.Result:
                    tcs.SetResult(SuperJsonSerializer.ConvertResult(result.Result, 
					tcs.GetType().GetGenericArguments()[0]));
                    break;
                default:
                    throw new Exception("Holy Moly!");
            }
        }

        protected async Task<object> GetObject(Socket socket)
        {
            var lenght = BitConverter.ToInt32(await socket.ReadBytes(4), 0);
            var packageData = Encoding.UTF8.GetString(await socket.ReadBytes(lenght));
            var result = mSerializer.Deserialize(packageData);
            return result;
        }

        protected byte[] GetBytes(object obj)
        {
            var json = mSerializer.Serialize(obj);
            return Encoding.UTF8.GetBytes(json);
        }
    }
}
