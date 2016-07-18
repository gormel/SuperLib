using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testes
{
    public interface ITestes
    {
        int Foo(int val);
        Task Bar();
        Task<int> Zar(string var);
        IEnumerable<string> GetStrings(byte count);
        ITestes Har();
        Guid GetId();
    }
}
