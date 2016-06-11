using System;
using System.Threading.Tasks;
using Testes;

namespace ServerTestes
{
    class TestesImpt : ITestes
    {
        public int Foo(int val)
        {
            Console.WriteLine($"TestesImpl.Foo({val});");
            //throw new Exception("Test Exception!");
            return val + 3;
        }

        public async Task Bar()
        {
            await Task.Delay(500);
            Console.WriteLine($"TestesImpl.Bar();");
        }

        public async Task<int> Zar(string var)
        {
            await Task.Delay(300);
            Console.WriteLine($"TestesImpl.Zar();");
            return int.Parse(var) + 1;
        }
    }
}
