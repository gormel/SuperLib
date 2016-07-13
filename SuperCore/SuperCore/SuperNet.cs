﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SuperJson;

namespace SuperCore
{
    public abstract class SuperNet : Super
    {
        //private readonly JsonSerializer mSerializer;
        private readonly SuperJsonSerializer mSerializer = new SuperJsonSerializer();

        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<CallResult>> mWaitingCalls 
            = new ConcurrentDictionary<Guid, TaskCompletionSource<CallResult>>();

        //Guid => TaskCompletitionSource
        internal readonly ConcurrentDictionary<Guid, dynamic> WaitingTasks 
            = new ConcurrentDictionary<Guid, dynamic>();

        protected SuperNet()
        { /*
            mSerializer = JsonSerializer.CreateDefault(new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                Converters = { new ReadTaskJsonConverter(this), new WriteTaskJsonConverter(this) }
            });*/
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

        protected void StartReadClient(Socket client)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var resultObj = await GetObject(client);
                    if (resultObj is CallInfo)
                    {
                        CallResult result;
                        try
                        {
                            result = Call((CallInfo)resultObj);
                        }
                        catch (Exception e)
                        {
                            result = new CallResult
                            {
                                CallID = ((CallInfo)resultObj).CallID,
                                Result = e,
                                Exception = true
                            };
                        }

                        var data = GetBytes(result);
                        await client.SendBytes(BitConverter.GetBytes(data.Length));
                        await client.SendBytes(data);
                    }
                    else if (resultObj is Result)
                    {
                        ReciveData((Result)resultObj);
                    }
                }
            }).ContinueWith(t =>
            {
                var ex = t.Exception;
            });
        }
        
        internal abstract void SendData(object info);

        protected void ReciveData(Result result)
        {
            dynamic res = result;
            ReciveData(res);
        }

        protected void ReciveData(CallResult result)
        {
            TaskCompletionSource<CallResult> tcs;
            mWaitingCalls.TryRemove(result.CallID, out tcs);
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
                    tcs.SetResult(SuperJsonSerializer.ConvertResult(result.Result, tcs.GetType().GetGenericArguments()[0]));
                    break;
                default:
                    throw new Exception("Holy Moly!");
            }
        }

        protected async Task<object> GetObject(Socket socket)
        {
            var lenght = BitConverter.ToInt32(await socket.ReadBytes(4), 0);
            var packageData = Encoding.UTF8.GetString(await socket.ReadBytes(lenght));
            Trace.WriteLine(packageData);
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
