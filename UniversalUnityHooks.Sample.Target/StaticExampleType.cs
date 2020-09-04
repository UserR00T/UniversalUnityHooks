using System;

namespace UniversalUnityHooks.Sample.Target
{
    public static class StaticExampleType
    {
        public static void StaticMethod(string awesomeString)
        {
            Console.WriteLine($"(static type) The awesome string value is: {awesomeString}");
        }
    }
}
