namespace UniversalUnityHooks.Logging
{
    public class LoggingSettings
    {
        /// <summary>
        /// Determines if the logging should parse '\n' characters and add spacing to the start of it.
        /// </summary>
        /// <value></value>
        public bool AddSpacingAfterNewLine { get; set; } = true;

        /// <summary>
        /// Adds the specified amount of spaces to the content to offset it from the id. <c>0</c> to disable.
        /// </summary>
        /// <value></value>
        public int MessagePadding { get; set; } = 2;

        /// <summary>
        /// If set to higher than <c>0</c>, will output <see cref="UniversalUnityHooks.Logging.Interfaces.ILogger.LogDebug(string)"/> messages. If set to something higher, will output values lower of equal to that provided with <see cref="UniversalUnityHooks.Logging.Interfaces.ILogger.LogDebug(string, int)"/>.
        /// </summary>
        /// <value></value>
        public int DebugLevel { get; set; } = 0;
    }
}