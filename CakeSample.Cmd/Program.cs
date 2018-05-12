using System;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var result = ClassLibrary1.Class1.SayHello("NoVa Code Camp");
            Console.WriteLine(result);
            Console.WriteLine("This dll was compiled at " + ReleaseDate);
        }

        public static DateTime ReleaseDate
        {
            get { return new FileInfo(typeof(Program).Assembly.Location).LastWriteTime; }
        }
    }
}
