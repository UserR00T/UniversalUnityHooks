namespace UniversalUnityHooks.PluginLoader
{
    /// <summary>
    /// Represents a single plugin author.
    /// </summary>
    public class PluginAuthor
    {
        /// <summary>
        /// The plugin author's name.
        /// </summary>
        /// <value></value>
        public string Name { get; set; }

        /// <summary>
        /// The position of the plugin author, eg 'Developer' or 'Designer'.
        /// </summary>
        /// <value></value>
        public string Function { get; set; }

        /// <summary>
        /// Represents a single plugin author.
        /// </summary>
        /// <param name="name">The plugin author's name.</param>
        /// <param name="function">The plugin author's function.</param>
        public PluginAuthor(string name, string function = "Developer")
        {
            Name = name;
            Function = function;
        }

        /// <summary>
        /// Returns the name and the function as one string.
        /// </summary>
        /// <returns>Returns 'Name: Function' as string.</returns>
        public override string ToString()
        {
            return $"{Name}: {Function}";
        }
    }
}