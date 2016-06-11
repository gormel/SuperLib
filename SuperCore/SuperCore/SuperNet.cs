﻿using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SuperCore
{
    public abstract class SuperNet : Super
    {
        private readonly JsonSerializer mSerializer = JsonSerializer.CreateDefault(new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All,
        });

        private readonly ConcurrentDictionary<Guid, TaskCompletionSource<CallResult>> mWaitingCalls 
            = new ConcurrentDictionary<Guid, TaskCompletionSource<CallResult>>();
        
        public override CallResult SendCall(CallInfo info)
        {
            var tcs = new TaskCompletionSource<CallResult>();
            mWaitingCalls.TryAdd(info.CallID, tcs);
            SendData(info);
            var method = info.GetMethodInfo();
            if (typeof (Task).IsAssignableFrom(method.ReturnType))
            {
                var taskResultType = method.ReturnType.GenericTypeArguments.SingleOrDefault() ?? typeof(object);
                var callAsyncMethod = typeof(SuperNet).GetMethod(nameof(CallAsyncMethod));
                return (CallResult)callAsyncMethod
                    .MakeGenericMethod(taskResultType)
                    .Invoke(this, new object[] { info, tcs.Task });
            }
            var result = tcs.Task.Result;
            ConvertResult(result, method.ReturnType);
            return result;
        }

        private CallResult CallAsyncMethod<T>(CallInfo callInfo, Task<CallResult> callTask)
        {
            var tcs = new TaskCompletionSource<T>();
            callTask.ContinueWith(t =>
            {
                if (t.IsCanceled)
                    tcs.SetCanceled();
                else if (t.IsFaulted)
                    tcs.SetException(t.Exception);
                else
                {
                    if (callInfo.GetMethodInfo().ReturnType == typeof(Task))
                    {
                        tcs.SetResult(default(T));
                    }
                    else
                    {
                        CallResult callResult = callTask.Result;
                        ConvertResult(callResult, typeof(T));
                        tcs.SetResult((T)callResult.Result);
                    }
                }

            });
            return new CallResult
            {
                CallID = callInfo.CallID,
                Result = tcs.Task
            };
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
                                Status = TaskCompletionStatus.NoTaskException
                            };
                        }

                        var task = result.Result as Task;
                        if (task != null)
                        {
                            try
                            {
                                await task;
                            }
                            catch {}
                            DoSmth(result, task);
                        }
                        
                        var data = GetBytes(result);
                        await client.SendBytes(BitConverter.GetBytes(data.Length));
                        await client.SendBytes(data);
                    }
                    else if (resultObj is CallResult)
                    {
                        ReciveData((CallResult)resultObj);
                    }
                }
            }).ContinueWith(t =>
            {
                var ex = t.Exception;
            });
        }

        private void DoSmth(CallResult result, Task task)
        {
            if (task.IsCanceled)
            {
                result.Status = TaskCompletionStatus.Canceled;
            }
            else if (task.IsFaulted)
            {
                result.Status = TaskCompletionStatus.Exception;
                result.Result = task.Exception;
            }
            else if (task.IsCompleted)
            {
                var resultProp = task.GetType().GetProperty("Result");
                var propResult = resultProp.GetValue(task);
                result.Status = TaskCompletionStatus.Result;
                result.Result = propResult;
            }
            else
            {
                throw new ArgumentException("It's Bad :(, task is not completed!!!");
            }
        }

        private void ConvertResult(CallResult result, Type methodType)
        {
            var resultData = result.Result;
            if (methodType != resultData.GetType())
            {
                if (resultData is IConvertible)
                    result.Result = Convert.ChangeType(result.Result, methodType);
            }
        }
        
        protected abstract void SendData(CallInfo info);

        protected void ReciveData(CallResult result)
        {
            TaskCompletionSource<CallResult> tcs;
            mWaitingCalls.TryRemove(result.CallID, out tcs);

            switch (result.Status)
            {
                case TaskCompletionStatus.Canceled:
                    tcs.SetCanceled();
                    break;
                case TaskCompletionStatus.NoTaskException:
                case TaskCompletionStatus.Exception:
                    tcs.SetException((Exception)result.Result);
                    break;
                case TaskCompletionStatus.NoTask:
                case TaskCompletionStatus.Result:
                    tcs.SetResult(result);
                    break;
                default:
                    throw new Exception("Holy moly!");
            }
        }

        protected async Task<object> GetObject(Socket socket)
        {
            var lenght = BitConverter.ToInt32(await socket.ReadBytes(4), 0);
            var packageData = Encoding.UTF8.GetString(await socket.ReadBytes(lenght));
            Trace.WriteLine(packageData);
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
