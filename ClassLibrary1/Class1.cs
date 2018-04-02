using System;

namespace ClassLibrary1
{
    public class Class1
    {
        public string Greeting {get; set; }

        public static string SayHello(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return "Hello Friend";
            }
            else
            {
                return "Hello " + name;
            }
        }

        public static string Serialize(Class1 class1)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(class1);
        }
    }
}
