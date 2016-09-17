using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using SuperJson.Objects;
using SuperJson.Parser;
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
        
        private readonly SuperJsonParser mParser = new SuperJsonParser();

        public string Serialize(object obj)
        {
            return mParser.Write(Serialize(obj, null));
        }

        public SuperToken Serialize(object obj, Type declaredType)
        {
            foreach (var customer in SerializeCustomers)
            {
                if (!customer.UseCustomer(obj, declaredType))
                    continue;

                return customer.Serialize(obj, declaredType, this);
            }

            var objType = obj.GetType();
            
            var result = new SuperObject();

            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var members = objType.GetMembers(bindingFlags);
            
            result.TypedValue.Add("$type", new SuperString(objType.AssemblyQualifiedName));
            
            foreach (var member in members)
            {
                var attrs = member.GetCustomAttributes(typeof (CompilerGeneratedAttribute));
                if (attrs.Any())
                    continue;
                attrs = member.GetCustomAttributes(typeof(DoNotSerialiseAttribute));
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

                result.TypedValue.Add(member.Name, Serialize(memberValue, declaredMemberType));
            }

            return result;
        }

        public object Deserialize(string json)
        {
            return Deserialize(mParser.Parse(json), null);
        }

        public object Deserialize(SuperToken obj, Type declaredType)
        {
            switch (obj.TokenType)
            {
                case SuperTokenType.Null:
                    return null;
                case SuperTokenType.Array:
                    var arr = (SuperArray)obj;
                    var resultArr = new object[arr.TypedValue.Length];
                    for (var i = 0; i < arr.TypedValue.Length; i++)
                    {
                        resultArr[i] = Deserialize(arr.TypedValue[i], null);
                    }
                    return resultArr;
                case SuperTokenType.Bool:
                case SuperTokenType.String:
                case SuperTokenType.Number:
                    return obj.Value;
                case SuperTokenType.Object:
                    foreach (var customer in DeserializeCustomers)
                    {
                        if (!customer.UseCustomer(obj, declaredType))
                            continue;
                        return customer.Deserialize(obj, this);
                    }

                    var resultObj = (SuperObject) obj;

                    var typeName = (resultObj.TypedValue["$type"] as SuperString)?.TypedValue;//todo: protected serialization constructor
                    if (string.IsNullOrEmpty(typeName))
                        throw new FormatException();
                    var type = Type.GetType(typeName, true);
                    var inst = Activator.CreateInstance(type);

                    var props = resultObj.TypedValue;
                    foreach (var prop in props)
                    {
                        if (prop.Key.StartsWith("$"))
                            continue;

                        var memInfo = type.GetMember(prop.Key, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)[0];
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
            if (result == null)
                return null;
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
