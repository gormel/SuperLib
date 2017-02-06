using System;
using System.Collections.Generic;
using System.Linq;
using SuperJson;
using SuperJson.Parser;

namespace JsonTestes
{
    class Program
	{
        private class Test1
        {
            private static readonly Lazy<Test1> mInstance = new Lazy<Test1>(() => new Test1());
            public static Test1 Instance => mInstance.Value;

            public int Field1;
            public int Property1 { get; set; }

            [DoNotSerialise]
            public int Property2 { get; set; }

            public Dictionary<int, int> DictProperty = new Dictionary<int, int>(); 
        }

        static void Main(string[] args)
        {
//            var result = "";
//            var className = "DelegateFuncWrapper";
//            for (int i = 1; i < 10; i++)
//            {
//                var genericTypesSplitted = Enumerable.Range(1, i).Select(n => $"T{n}").ToList();
//                var genericTypes = genericTypesSplitted.Aggregate((a, b) => $"{a}, {b}");
//                var argsDeclaration = genericTypesSplitted.Select(t => $"{t} arg{t.Substring(1)}").Aggregate((a, b) => $"{a}, {b}");
//                var argsNames = Enumerable.Range(1, i).Select(n => $"arg{n}").Aggregate((a, b) => $"{a}, {b}");
//                var decl = $@"
//public class {className}<{genericTypes}, TResult>
//{{
//	private readonly Delegate mSubject;

//	public {className}(Delegate subject)
//	{{
//        mSubject = subject;
//	}}

//    public TResult Invoke({argsDeclaration})
//    {{
//        return (TResult)mSubject.DynamicInvoke({argsNames});
//    }}
//}}
//";
//                result += $"{decl}{Environment.NewLine}{Environment.NewLine}";
//            }

            var parser = new SuperJsonParser();
            var serializer = new SuperJsonSerializer();
            var json = serializer.Serialize(new[] { 1, 2.7, 3, 5 });
            var list = new List<int>() { 0, 5, 8, 4 };
            var json1 = serializer.Serialize(list);
            var cl = new Test1() { Field1 = 5, Property1 = 7, Property2 = 9 };
            var json2 = serializer.Serialize(cl);

            var parsed = parser.Parse(json1);

            var unparsed = parser.Write(parsed);

            var arr1 = serializer.Deserialize(json);
            var list1 = serializer.Deserialize(json1);
            var cl1 = serializer.Deserialize(json2);

            Console.ReadLine();
        }
    }
}
