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
                var result = testesImpl.Foo(5);
                Console.WriteLine(result);
            }
        }
    }
}
