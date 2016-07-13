using System;
using SuperCore;
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
            while (Console.ReadLine() != "Exit")
            {
                var resultTask = testesImpl.Zar("10");
                resultTask.ContinueWith(t => Console.WriteLine($"testesImpl.Zar({t.Result});"));
                /*
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
