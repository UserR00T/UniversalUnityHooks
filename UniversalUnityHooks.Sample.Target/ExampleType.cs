using System;

namespace UniversalUnityHooks.Sample.Target
{
    public class ExampleType
    {
        private int privateInt;

        public int publicInt;

        public int Add(int number, int number2)
        {
            return number + number2;
        }

        public void OutputValue(string value)
        {
            Console.WriteLine($"The output is: {value}");
        }

        public void InterceptTest()
        {
            Console.WriteLine("You should not see this message if this method has been injected");
        }

        public int InterceptAndReturnTest(string someString)
        {
            return 42;
        }

        public static void StaticMethod(string awesomeString)
        {
            Console.WriteLine($"The awesome string value is: {awesomeString}");
        }
    }
}
