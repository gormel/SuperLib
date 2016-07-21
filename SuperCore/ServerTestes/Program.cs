using System;
using SuperCore;
using SuperCore.Core;
using Testes;

namespace ServerTestes
{
    class Program
    {
        static void Main(string[] args)
        {
			Console.WriteLine ("Server Testes v0.000001");
            var impl = new TestesImpl();
            var server = new SuperServer();
            server.StartListen(55666);
            server.Register<ITestes>(impl);
            while (Console.ReadLine() != "Exit") ;
        }
    }
}

