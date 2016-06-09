using System;
using SuperCore;
using Testes;

namespace ServerTestes
{
    class Program
    {
        static void Main(string[] args)
        {
            var impl = new TestesImpt();
            var server = new SuperServer();
            server.StartListen(666);
            server.Register<ITestes>(impl);
            while (Console.ReadLine() != "Exit") ;
        }
    }
}

