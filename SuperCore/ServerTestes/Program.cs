using System;
using SuperCore.Core;
using Testes;
using System.Threading.Tasks;
using System.Threading;

namespace ServerTestes
{
    class Program
    {
        static void Main(string[] args)
        {
			Console.WriteLine ("Server Testes v0.000001");
            var impl = new TestesImpl();
            var server = new SuperServer();

            var stopSource = new CancellationTokenSource();
            Task listening = server.Listen(5566, stopSource.Token);
            server.Register<ITestes>(impl);

            while (Console.ReadLine() != "Exit") { }

            stopSource.Cancel();
            listening.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    Console.Error.WriteLine(t.Exception);
                    Console.ReadLine();
                }
            }).Wait();
        }
    }
}

