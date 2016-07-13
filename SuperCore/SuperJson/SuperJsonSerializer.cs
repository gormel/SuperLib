using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using SuperJson.SerializeCustomers;

namespace SuperJson
{
    public class SuperJsonSerializer
    {
        public List<SerializeCustomer> SerializeCustomers { get; } = new List<SerializeCustomer>
        {
            new NullSerializeCustomer(),
            new PrimetiveSerializeCustomer(),
            new StringSerializeCustomer(),
            new ArraySerializeCustomer()
        };
        public List<DeserializeCustomer> DeserializeCustomers { get; } = new List<DeserializeCustomer>();
        
        public string Serialize(object obj)
        {
            return Serialize(obj, null);
        }

        private string Serialize(object obj, Type declaredType)
        {
            foreach (var customer in SerializeCustomers)
            {
                if (!customer.UseCustomer(obj, declaredType))
                    continue;

                return customer.Serialize(obj, this);
            }

            var objType = obj.GetType();
            
            string result = "";

            var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var members = objType.GetMembers(bindingFlags);

            result += "{";

            result += $"\"$type\":\"{objType.AssemblyQualifiedName}\",";

            foreach (var member in members)
            {
                var attrs = member.GetCustomAttributes(typeof (CompilerGeneratedAttribute));
                if (attrs.Any())
                    continue;
                object memberValue = null;
                Type declaredMemberType = null;
                if (member.MemberType == MemberTypes.Field)
                {
                    var fieldInfo = (FieldInfo)member;
                    memberValue = fieldInfo.GetValue(obj);
                    declaredMemberType = fieldInfo.FieldType;
                }
                if (member.MemberType == MemberTypes.Property)
                {
                    var propInfo = (PropertyInfo)member;
                    if (!propInfo.CanRead || !propInfo.CanWrite)
                        continue;
                    try
                    {
                        memberValue = propInfo.GetValue(obj);
                    }
                    catch
                    {
                    }
                    declaredMemberType = propInfo.PropertyType;
                }

                if (memberValue == null)
                    continue;

                result += $"\"{member.Name}\":{Serialize(memberValue, declaredMemberType)},";
            }

            result = result.Substring(0, result.LastIndexOf(",", StringComparison.Ordinal));

            result += "}";

            return result;
        }

        public object Deserialize(string json)
        {
            return Deserialize(JToken.Parse(json), null);
        }

        private object Deserialize(JToken obj, Type declaredType)
        {
            switch (obj.Type)
            {
                case JTokenType.Array:
                    var arr = (JArray)obj;
                    var resultArr = new object[arr.Count];
                    for (var i = 0; i < arr.Count; i++)
                    {
                        resultArr[i] = Deserialize(arr[i], null);
                    }
                    return resultArr;
                case JTokenType.Boolean:
                case JTokenType.String:
                case JTokenType.Integer:
                case JTokenType.Float:
                    var decVal = (JValue)obj;
                    return decVal.Value;
                case JTokenType.Object:
                    foreach (var customer in DeserializeCustomers)
                    {
                        if (!customer.UseCustomer(obj, declaredType))
                            continue;
                        return customer.Deserialize(obj, this);
                    }

                    var typeName = obj["$type"].ToString();
                    if (string.IsNullOrEmpty(typeName))
                        throw new FormatException();
                    var type = Type.GetType(typeName, true);
                    var inst = Activator.CreateInstance(type);

                    var props = obj.Children().Cast<JProperty>();
                    foreach (var prop in props)
                    {
                        if (prop.Name.StartsWith("$"))
                            continue;

                        var memInfo = type.GetMember(prop.Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)[0];
                        if (memInfo.MemberType == MemberTypes.Field)
                        {
                            var fieldInfo = (FieldInfo)memInfo;
                            var propValue = Deserialize(prop.Value, fieldInfo.FieldType);
                            fieldInfo.SetValue(inst, ConvertResult(propValue, fieldInfo.FieldType));
                        }

                        if (memInfo.MemberType == MemberTypes.Property)
                        {
                            var propInfo = (PropertyInfo)memInfo;
                            var propValue = Deserialize(prop.Value, propInfo.PropertyType);
                            propInfo.SetValue(inst, ConvertResult(propValue, propInfo.PropertyType));
                        }
                    }

                    return inst;
                default:
                    throw new FormatException();
            }
        }

        public static object ConvertResult(object result, Type methodType)
        {
            if (result.GetType().IsArray && methodType.IsArray)
            {
                var arr = (Array)result;
                var resultArray = (Array)Activator.CreateInstance(methodType, arr.Length);
                for (var i = 0; i < arr.Length; i++)
                {
                    resultArray.SetValue(ConvertResult(arr.GetValue(i), methodType.GetElementType()), i);
                }
                return resultArray;
            }
            if (methodType != result.GetType())
            {
                if (result is IConvertible)
                    return Convert.ChangeType(result, methodType);
            }
            return result;
        }
    }
}
