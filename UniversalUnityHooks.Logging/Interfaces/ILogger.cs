using UniversalUnityHooks.Logging.Models;

namespace UniversalUnityHooks.Logging.Interfaces
{
    public interface ILogger
    {
        string Id { get; }

        LoggerSettings Settings { get; }

        void Log(LogLevel level, string message);

        void LogDebug(string message);

        void LogDebug(string message, int verbosity);

        void LogInformation(string message);

        void LogWarning(string message);

        void LogError(string message);

        void LogCritical(string message);

        void NewLine();
    }
}