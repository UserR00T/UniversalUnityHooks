using System;
using System.Linq;
using System.Reflection;

namespace UniversalUnityHooks.PluginLoader
{
    /// <summary>
    /// The plugin loader class. This type will load in all plugins from a Assembly context.
    /// </summary>
    internal class Loader
    {
        /// <summary>
        /// A list of loaded in plugins.
        /// </summary>
        /// <returns></returns>
        public Plugins Plugins { get; } = new Plugins();

        /// <summary>
        /// Loads in a single plugin by assembly type, which finds and instantiates the plugin instance. 
        /// </summary>
        /// <param name="assembly">The assembly to search in.</param>
        internal void LoadAssembly(Assembly assembly)
        {
            var type = assembly.GetExportedTypes().FirstOrDefault(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(Plugin)));
            if (type == null)
            {
                // TODO: error logging
                return;
            }
            var plugin = (Plugin)Activator.CreateInstance(type);
            Plugins.LoadedPlugins.Add(plugin.Data.Name, plugin);
        }

        /// <summary>
        /// Initializes all plugins.
        /// </summary>
        public void InitializeAll()
        {
            foreach (var plugin in Plugins.LoadedPlugins.Values)
            {
                plugin.OnInitialzed();
                plugin.State = PluginState.Initialized;
            }
        }

        /// <summary>
        /// Terminates all plugins.
        /// </summary>
        public void TerminateAll()
        {
            foreach (var plugin in Plugins.LoadedPlugins.Values)
            {
                plugin.OnTerminated();
                plugin.State = PluginState.Terminated;
            }
        }
    }
}