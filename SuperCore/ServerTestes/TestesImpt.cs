using System;
using System.Collections.Generic;
using System.Linq;
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
            Console.WriteLine($"start TestesImpl.Zar();");
            await Task.Delay(1000);
            Console.WriteLine($"end TestesImpl.Zar();");
            return int.Parse(var) + 1;
        }

        public IEnumerable<string> GetStrings(byte count)
        {
            return (from n in Enumerable.Range(0, count)
                   select new string('$', n)).ToArray();
        }
    }
}
