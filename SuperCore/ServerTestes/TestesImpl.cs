using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Testes;

namespace ServerTestes
{
	class TestesImpl : ITestes
    {
        private Guid mId = Guid.NewGuid();
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
                   select new string('$', n));
        }

        public ITestes Har()
        {
            return this;
        }

        public Guid GetId()
        {
			Act?.Invoke();
            return mId;
        }

        public int TestesProp { get; set; }
        public event Action Act;

		public event Func<string> Act1;
	    public event Func<int, double, string> Act2;


	    public string Get1 (int input, double input2)
		{
			return Act2?.Invoke (input, input2);
		}
    }
}
