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
            if (Settings.AddSpacingAfterNewLine)
            {
                message = message.Replace("\n", $"\n{new string(' ', Math.Max(_minPrefixWidth + Settings.MessagePadding, prefix.Length))}");
            }
            lock (_lock)
            {
                Util.WriteColored(prefix.PadRight(Math.Max(_minPrefixWidth, prefix.Length), ' '), color);
                Util.WriteColored(id, ConsoleColor.DarkGray);
                Console.WriteLine();
                Console.Write(new string(' ', Math.Max(_minPrefixWidth + Settings.MessagePadding, prefix.Length)));
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

        public void LogDebug(string message)
        {
            if (Settings.DebugVerbosity < 1)
            {
                return;
            }
            Log("debug", ConsoleColor.DarkGray, Id, message);
        }

        public void LogDebug(string message, int verbosity)
        {
            if (Settings.DebugVerbosity < verbosity)
            {
                return;
            }
            LogDebug(message);
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