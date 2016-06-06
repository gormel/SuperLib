using System;
using SuperCore;

namespace Testes
{
    public interface ITestes
    {
        int add(int count);
    }

    class Program
    {
        static void Main(string[] args)
        {
            var superTest = new SuperTest();
            superTest.Register<ITestes>(new Testes());

            var generated = superTest.GetInstance<ITestes>();

            Console.ReadLine();
        }
    }
}
