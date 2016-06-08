using System;
using SuperCore;

namespace Testes
{
    public interface ITestes
    {
        int add(int count);
        void hren(int count);
    }

    class Program
    {
        static void Main(string[] args)
        {
            var superTest = new SuperTest();
            var testes = new Testes();
            superTest.Register<ITestes>(testes);

            var generated = superTest.GetInstance<ITestes>();

            var result = generated.add(5);
            generated.hren(4);

            Console.ReadLine();
        }
    }
}
