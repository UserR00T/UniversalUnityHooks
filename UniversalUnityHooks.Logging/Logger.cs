using System;
using UniversalUnityHooks.Logging.Interfaces;
using UniversalUnityHooks.Logging.Models;
using UniversalUnityHooks.Logging.Utility;

namespace UniversalUnityHooks.Logging
{
    public class Logger : ILogger
    {
        private readonly object _lock = new object();

        public virtual string Id { get; }

        public LoggerSettings Settings { get; }

        private int _minPrefixWidth = "debug: ".Length;

        public Logger()
        {
        }

        public Logger(LoggerSettings settings) : this()
        {
            Settings = settings ?? new LoggerSettings();
        }

        public Logger(string id, LoggerSettings settings = null) : this(settings)
        {
            Id = id;
        }

        protected virtual void Log(string prefix, ConsoleColor color, string id, string message)
        {
            prefix += ": ";
            // TODO: Make this a setting
            message = message.Replace("\n", $"\n{new string(' ', Math.Max(_minPrefixWidth + 2, prefix.Length))}");
            lock (_lock)
            {
                Util.WriteColored(prefix.PadRight(Math.Max(_minPrefixWidth, prefix.Length), ' '), color);
                Util.WriteColored(id, ConsoleColor.DarkGray);
                Console.WriteLine();
                // TODO: Move hardcoded number to property
                Console.Write(new string(' ', Math.Max(_minPrefixWidth + 2, prefix.Length)));
                Console.WriteLine(message);
            }
        }

        public void Log(LogLevel level, string message)
        {
            // TODO: Not ideal, figure out a better way
            switch (level)
            {
                case LogLevel.Critical:
                    LogCritical(message);
                    break;
                    
                case LogLevel.Debug:
                    LogDebug(message);
                    break;

                case LogLevel.Error:
                    LogError(message);
                    break;

                case LogLevel.Information:
                    LogInformation(message);
                    break;

                case LogLevel.Warning:
                    LogWarning(message);
                    break;
            }
        }

        public void LogCritical(string message)
        {
            Log("crit", ConsoleColor.DarkRed, Id, message);
        }

        // TODO: Show debug depending on variable set
        public void LogDebug(string message)
        {
            Log("debug", ConsoleColor.DarkGray, Id, message);
        }

        public void LogDebug(string message, int verbosity)
            throw new NotImplementedException();
        }

        public void LogError(string message)
        {
            Log("error", ConsoleColor.Red, Id, message);
        }

        public void LogInformation(string message)
        {
            Log("info", ConsoleColor.Green, Id, message);
        }

        public void LogWarning(string message)
        {
            Log("warn", ConsoleColor.Yellow, Id, message);
        }

        public void NewLine()
        {
            Console.WriteLine();
        }
    }
}