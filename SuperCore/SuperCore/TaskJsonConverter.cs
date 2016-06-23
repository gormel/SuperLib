using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SuperCore
{
    class TaskJsonConverter : JsonConverter
    {
        private readonly SuperNet mSuper;

        public TaskJsonConverter(SuperNet super)
        {
            mSuper = super;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is Task)
            {
                var typedValue = value as Task;

                var resultType = value.GetType().GetGenericArguments()[0];
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

                writer.WriteStartObject();

                writer.WritePropertyName("$type");
                writer.WriteValue("TaskWrapper");

                writer.WritePropertyName("ID");
                writer.WriteValue(id);

                writer.WritePropertyName("ResultType");
                writer.WriteValue(resultType.FullName);

                writer.WriteEndObject();
                return;
            }
            
            JObject.FromObject(value, serializer).WriteTo(writer, this);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var typeToken = token.SelectToken("$type", false);
            if (typeToken?.ToString() == "TaskWrapper")
            {
                var obj = JObject.Parse(token.ToString());
                var id = Guid.Parse(obj["ID"].ToString());
                var resultType = Type.GetType(obj["ResultType"].ToString());
                var tcsType = typeof (TaskCompletionSource<>).MakeGenericType(resultType);
                dynamic tcs = Activator.CreateInstance(tcsType);
                mSuper.WaitingTasks[id] = tcs;
                return tcs.Task;
            }
            return token.ToObject(reader.ValueType, serializer);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object) || typeof(Task).IsAssignableFrom(objectType);
        }
    }
}
