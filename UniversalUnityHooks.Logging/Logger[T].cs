using UniversalUnityHooks.Logging.Interfaces;

namespace UniversalUnityHooks.Logging
{
    public class Logger<T> : Logger, ILogger<T>
    {
        public override string Id { get; } = typeof(T).FullName;
    }
}
