using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SuperCore
{
    class InterfaceJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JObject.FromObject(value, serializer).WriteTo(writer, this);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            return token.ToObject(reader.ValueType, serializer);
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof (object) == objectType || objectType.IsInterface;
        }
    }
}
