using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testes;

namespace ServerTestes
{
    class TestesImpt : ITestes
    {
        public int Foo(int val)
        {
            Console.WriteLine($"TestesImpl.Foo({val});");
            return val + 3;
        }
    }
}
