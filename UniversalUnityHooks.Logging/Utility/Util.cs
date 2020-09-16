using System;

namespace UniversalUnityHooks.Logging.Utility
{
    public static class Util
    {
        public static void WriteColored(string message, ConsoleColor color)
        {
            var resetColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(message);
            Console.ForegroundColor = resetColor;
        }
    }
}