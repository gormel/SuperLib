using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using SuperJson;
using SuperJson.Objects;
using SuperJson.Parser;

namespace JsonTestes
{
    class Program
    {

        static void Main(string[] args)
        {
            var parser = new SuperJsonParser();

            var dir = args[0];

            parser.Parse("-0.1");

            var files = Directory.EnumerateFiles(dir, "*.json");

            foreach (var file in files)
            {
                var fname = Path.GetFileName(file) ?? "";
                if (fname.StartsWith("y_"))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else if (fname.StartsWith("n_"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.WriteLine($"Parsing: {file}");
                SuperToken result = null;
                try
                {
                    result = parser.Parse(File.ReadAllText(file));
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e);
                }
                Console.ForegroundColor = result == null ? ConsoleColor.Red : ConsoleColor.Green;
                Console.WriteLine($"Parsing result: {(result != null ? "Success" : "Failed")}");
            }

            Console.ReadLine();
        }
    }
}
