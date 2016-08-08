using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SuperJson;
using System.Reflection;

namespace JsonTestes
{
    class Program
	{
        static void Main(string[] args)
        {
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
