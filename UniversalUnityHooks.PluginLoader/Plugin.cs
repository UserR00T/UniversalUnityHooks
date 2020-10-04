using UniversalUnityHooks.Logging;
using UniversalUnityHooks.Logging.Interfaces;

namespace UniversalUnityHooks.PluginLoader
{
    /// <summary>
    /// This class represents a plugin. This is an abstract type which should be implemented in each of your plugins.
    /// </summary>
    public abstract class Plugin
    {
        /// <summary>
        /// Extra plugin data. Must be populated.
        /// </summary>
        /// <value></value>
        public abstract PluginData Data { get; }

        /// <summary>
        /// A simple preconfigured logger for the plugin instance to use.
        /// </summary>
        /// <value></value>
        public ILogger Logger { get; }

        /// <summary>
        /// The current plugin state. While a plugin may be loaded in for the UniversalUnityHooks' context, this still may imply the plugin is currently not actually active. This is determined by this property.
        /// </summary>
        /// <value></value>
        public PluginState State { get; internal set; }

        /// <summary>
        /// The main plugin class constructor.
        /// </summary>
        public Plugin()
        {
            var name = GetType().Assembly.GetName();
            Logger = new Logger($"Plugin::{name.Name}@{name.Version}");
        }

        /// <summary>
        /// This event occurs once the plugin gets loaded. You may do any IO in here to prepare the plugin.
        /// </summary>
        public abstract void OnInitialzed();

        /// <summary>
        /// This event occurs once the plugin gets uploaded. This happens when the plugin gets reloaded, and when the plugin gets stopped.
        /// </summary>
        public abstract void OnTerminated();
    }
}
