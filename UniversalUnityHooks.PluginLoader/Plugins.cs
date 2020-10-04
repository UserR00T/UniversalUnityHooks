using System.Collections.Generic;

namespace UniversalUnityHooks.PluginLoader
{
    /// <summary>
    /// A list of currently loaded in plugins.
    /// </summary>
    public class Plugins
    {
        /// <summary>
        /// The actual dictionary of loaded in plugins. This should not be mutated directly, but instead; use the extra methods defined in this type.
        /// </summary>
        /// <typeparam name="string">The plugin name. Grabbed from <see cref="Plugin.Data"/>.</typeparam>
        /// <typeparam name="Plugin">The actual plugin class instance.</typeparam>
        /// <returns></returns>
        public Dictionary<string, Plugin> LoadedPlugins { get; } = new Dictionary<string, Plugin>();

        /// <summary>
        /// Wrapper for TryGetValue. Returns false if not found, and true if found with the out param set.
        /// </summary>
        /// <param name="name">The plugin name to search for.</param>
        /// <param name="plugin">If found, sets this value to the found value.</param>
        /// <returns>Returns true if found, else false.</returns>
        public bool TryGetPlugin(string name, out Plugin plugin)
        {
            return LoadedPlugins.TryGetValue(name, out plugin);
        }

        /// <summary>
        /// Same as <see cref="Plugins.TryGetPlugin(string, out Plugin)"/>, but direct return type instead of bool.
        /// </summary>
        /// <param name="name">The plugin name to search for.</param>
        /// <returns>Returns the plugin instance.</returns>
        public Plugin GetPlugin(string name)
        {
            if (!TryGetPlugin(name, out var plugin))
            {
                return null;
            }
            return plugin;
        }
    }
}