namespace UniversalUnityHooks.PluginLoader
{
    /// <summary>
    /// The valid states a plugin can be in.
    /// </summary>
    public enum PluginState
    {
        /// <summary>
        /// The plugin state is not yet known or is invalid.
        /// </summary>
        Unknown,

        /// <summary>
        /// The plugin has been disabled or is not yet initialized.
        /// </summary>
        Terminated,

        /// <summary>
        /// The plugin has been initialized correctly and is serving requests.
        /// </summary>
        Initialized,
    }
}