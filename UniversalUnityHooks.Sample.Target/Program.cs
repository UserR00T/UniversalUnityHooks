using System;

namespace UniversalUnityHooks.Sample.Target
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("UniversalUnityHooks.Sample.Target initialized");
            var exampleType = new ExampleType();
            var output = exampleType.Add(5, 10);
            Console.WriteLine($"Output: {output} / Expected: {5 + 10}");
            exampleType.OutputValue("[INFO] This is a test info message. /Hopefully/ this gets written to console!");
            exampleType.OutputValue("[ERROR] This is a error message. Something very bad has happened. Yes it has.");
        }
    }
}
