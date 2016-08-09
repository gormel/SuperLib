using System;
using SuperCore;
using SuperCore.Core;
using Testes;

namespace ClientTestes
{
    class Program
    {
        static void Main(string[] args)
        {
			Console.WriteLine ("Client Testes a0.00001");
            var client = new SuperClient();
            client.Connect("127.0.0.1", 5566);
            var testesImpl = client.GetInstance<ITestes>();
            testesImpl.Act += () => Console.WriteLine("Action!");
			//testesImpl.Act1 += () => "STROKA";
            while (Console.ReadLine() != "Exit")
            {
				Console.WriteLine (testesImpl.GetId ());
                int a = 6;
                /*
                var resultTask = testesImpl.Zar("10");
                resultTask.ContinueWith(t => Console.WriteLine($"testesImpl.Zar({t.Result});"));
                try
                {
                    var result = testesImpl.GetStrings(6);
                    Console.WriteLine($"{string.Join(", ", result)}");
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e);
                }
                */

            }
        }
    }
}
