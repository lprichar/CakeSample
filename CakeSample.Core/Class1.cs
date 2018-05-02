using System;

namespace ClassLibrary1
{
    public class Class1
    {
        public static string SayHello(string name) 
        {
            if (string.IsNullOrEmpty(name))
                return "Hello Friend";
            else
                return "Hello " + name;
        }
    }
}
