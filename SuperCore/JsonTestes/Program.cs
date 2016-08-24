using System;
using System.Collections.Generic;
using System.Linq;
using SuperJson;

namespace JsonTestes
{
    class Program
	{
        static void Main(string[] args)
        {
            var result = "";
            var className = "DelegateFuncWrapper";
            for (int i = 1; i < 10; i++)
            {
                var genericTypesSplitted = Enumerable.Range(1, i).Select(n => $"T{n}").ToList();
                var genericTypes = genericTypesSplitted.Aggregate((a, b) => $"{a}, {b}");
                var argsDeclaration = genericTypesSplitted.Select(t => $"{t} arg{t.Substring(1)}").Aggregate((a, b) => $"{a}, {b}");
                var argsNames = Enumerable.Range(1, i).Select(n => $"arg{n}").Aggregate((a, b) => $"{a}, {b}");
                var decl = $@"
public class {className}<{genericTypes}, TResult>
{{
	private readonly Delegate mSubject;

	public {className}(Delegate subject)
	{{
        mSubject = subject;
	}}

    public TResult Invoke({argsDeclaration})
    {{
        return (TResult)mSubject.DynamicInvoke({argsNames});
    }}
}}
";
                result += $"{decl}{Environment.NewLine}{Environment.NewLine}";
            }

            var serializer = new SuperJsonSerializer();
            var json = serializer.Serialize(new[] { 1, 2.7, 3, 5 });
            var list = new List<int>() { 0, 5, 8, 4 };
            var json1 = serializer.Serialize(list);

            var arr1 = serializer.Deserialize(json);
            var list1 = serializer.Deserialize(json1);

            Console.ReadLine();
        }
    }
}
