using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CircuitBreaker;

namespace SampleConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var testData = new List<Test>();
            testData.Add(new Test { Id = 1, Desc = "Fu" });
            testData.Add(new Test { Id = 2, Desc = "bar" });

             
        }

        static void someMethod()
        {
            Console.WriteLine("...crud.");
            throw new Exception("Holy cow, an error!");
        }
    }
}
