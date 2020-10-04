using System.Collections.Generic;

namespace UniversalUnityHooks.PluginLoader
{
    /// <summary>
    /// Metadata attached to a plugin.
    /// </summary>
    public class PluginData
    {
        /// <summary>
        /// The plugin name.
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// The plugin description.
        /// </summary>
        /// <value></value>
        public string Description { get; set; }

        /// <summary>
        /// The plugin website.
        /// </summary>
        /// <value></value>
        public string Website { get; set; }

        /// <summary>
        /// The plugin git link. Can be both a git ssh link (git@github.com:user/repo), or a link to the https (github.com/user/repo).
        /// </summary>
        /// <value></value>
        public string Git { get; set; }

        /// <summary>
        /// A list of plugin authors. Can be used to display contributors.
        /// </summary>
        /// <typeparam name="PluginAuthor"></typeparam>
        /// <returns></returns>
        public List<PluginAuthor> Authors { get; set; } = new List<PluginAuthor>();

        /// <summary>
        /// Returns the name and description (if not null) as a string.
        /// </summary>
        /// <returns>Returns the <see cref="Name"/> and <see cref="Description"/> as one string. 'Name: Description'. If <see cref="Description"/> is null, will return just the <see cref="Name"/>.</returns>
        public override string ToString()
        {
            var name = Name;
            if (Description != null)
            {
                name += $": {Description}";
            }
            return name;
        }
    }
}