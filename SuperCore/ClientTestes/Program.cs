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
            var client = new SuperClient();
            client.Connect("127.0.0.1", 666);
            var testesImpl = client.GetInstance<ITestes>();
            testesImpl.Act += () => Console.WriteLine("Action!");
            while (Console.ReadLine() != "Exit")
            {
                var result = testesImpl.Har();
                var result1 = result.Har();
                var resultId = result.GetId();
                var resultId1 = result1.GetId();
                result.TestesProp = 5;
                var five = result1.TestesProp;
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
