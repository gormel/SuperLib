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
using SuperCore.Wrappers;
using SuperCore.Async.SyncContext;
using SuperCore.Async;

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

        private readonly AsyncLock mSendLock = new AsyncLock();

        protected SuperNet(SuperSyncContext context = null)
            : base(context)
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
            var method = info.GetMethodInfo();
            for (int i = 0; i < info.Args.Length; i++)
            {
                var declaredType = method.GetParameters()[i].ParameterType;
                var declarationWrapper = new DeclarationWrapper
                {
                    Instance = info.Args[i],
                    TypeName = declaredType.AssemblyQualifiedName
                };
                info.Args[i] = declarationWrapper;
            }

            SendData(info);
            mContext.Wait(tcs.Task);
            var result = tcs.Task.Result;
            result.Result = SuperJsonSerializer.ConvertResult(result.Result, method.ReturnType);
            return result;
        }

        public override void SendDestroy(string typeName, Guid classID)
        {
            SendData(new CallDestroyInfo
            {
                ClassID = classID,
                TypeName = typeName
            });
        }

        protected async Task ReadClient(Socket client, CancellationToken stop = default(CancellationToken))
        {
            try
            {
                while (true)
                {
                    stop.ThrowIfCancellationRequested();
                    var resultObj = await GetObject(client).ConfigureAwait(false);
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

        protected async Task SendByteArray(Socket client, byte[] data)
        {
            using (await mSendLock.Lock())
            {
                await client.SendBytes(BitConverter.GetBytes(data.Length));
                await client.SendBytes(data);
            }
        }

        private async Task ProcessResult(Socket client, object resultObj)
        {
            if (resultObj is Call)
            {
                var send = await ReciveCall((Call) resultObj).ConfigureAwait(false);
                if (!send.Item1)
                    return;

                var data = GetBytes(send.Item2);
                await SendByteArray(client, data);
            }
            else if (resultObj is Result)
            {
                ReciveData((Result) resultObj);
            }
        }

        internal abstract void SendData(object info);

        protected abstract void ClientDisconnected(Socket client);

		Task<Tuple<bool, Result>> ReciveCall(Call info)
		{
			dynamic call = info;
			return ReciveCall(call);
		}

        private Task<Tuple<bool, Result>> ReciveCall(CallDestroyInfo info)
        {
            object notUsed;
            if (info.ClassID != Guid.Empty)
            {
                mIdRegistred.TryRemove(info.ClassID, out notUsed);
            }
            else
            {
                mRegistred.TryRemove(info.TypeName, out notUsed);
            }
            return Task.FromResult(Tuple.Create(false, (Result)null));
        }

		async Task<Tuple<bool, Result>> ReciveCall(CallInfo info)
		{
		    Result result;
			if (info.ClassID != Guid.Empty && !mIdRegistred.ContainsKey (info.ClassID))
				return Tuple.Create(false, (Result)null);
			if (info.ClassID == Guid.Empty && !mRegistred.ContainsKey (info.TypeName))
				return Tuple.Create(false, (Result)null);
			try
			{
				result = await Task.Run(() => Call(info)).ConfigureAwait(false);
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
			return Tuple.Create(true, result);
		}

        void ReciveData(Result result)
        {
            dynamic res = result;
            ReciveData(res);
        }

        void ReciveData(CallResult result)
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

        void ReciveData(TaskResult result)
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
            var lenght = BitConverter.ToInt32(await socket.ReadBytes(4).ConfigureAwait(false), 0);
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