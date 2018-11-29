using System;
using System.Collections.Generic;

namespace UniversalUnityHooks
{
    public static partial class Util
    {
        public static class ConsoleHelper
        {
            struct MessageTypeData
            {
                public MessageTypeData(ConsoleColor color, string text)
                {
                    this.color = color;
                    this.text = text;
                }
                public ConsoleColor color;
                public string text;
            }
            public enum MessageType
            {
                None,
                Error,
                Info,
                Warning,
                Success,
                Wait
            }
            static readonly Dictionary<MessageType, MessageTypeData> MessageTypes = new Dictionary<MessageType, MessageTypeData>
            {
                { MessageType.None, new MessageTypeData(ConsoleColor.White, "NONE") },
                { MessageType.Info, new MessageTypeData(ConsoleColor.DarkCyan, "INFO") },
                { MessageType.Error, new MessageTypeData(ConsoleColor.Red, "ERROR") },
                { MessageType.Warning, new MessageTypeData(ConsoleColor.DarkYellow, "WARNING") },
                { MessageType.Success, new MessageTypeData(ConsoleColor.DarkGreen, "SUCCESS") },
                { MessageType.Wait, new MessageTypeData(ConsoleColor.DarkMagenta, "WAIT") }
            };

            public static void WriteLine(string message, ConsoleColor BackgroundColor) => WriteLine(message, Console.ForegroundColor, BackgroundColor);
            public static void WriteLine(string message, ConsoleColor ForegroundColor, ConsoleColor BackgroundColor)
            {
                Console.BackgroundColor = BackgroundColor;
                Console.ForegroundColor = ForegroundColor;
                Console.WriteLine(message);
                Console.ResetColor();
            }
            public static void Write(string message, ConsoleColor BackgroundColor) => Write(message, Console.ForegroundColor, BackgroundColor);
            public static void Write(string message, ConsoleColor ForegroundColor, ConsoleColor BackgroundColor)
            {
                Console.BackgroundColor = BackgroundColor;
                Console.ForegroundColor = ForegroundColor;
                Console.Write(message);
                Console.ResetColor();
            }

            public static void WriteMessage(string message) => WriteMessage(MessageType.Info, message);
            public static void WriteMessage(MessageType type, string message)
            {
                var messageType = MessageTypes[type];
                Console.BackgroundColor = messageType.color;
                Console.Write(" " + messageType.text.PadRight(8));
                Console.ResetColor();
                Console.WriteLine(" " + message);
            }
        }

    }
}
