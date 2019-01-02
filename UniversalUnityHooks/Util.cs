using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UniversalUnityHooks.Attributes;
using static UniversalUnityHooks.AttributesHelper;

namespace UniversalUnityHooks
{
    public static partial class Util
    {
        public static string GetManagedDirectory()
        {
            foreach (string directory in Directory.GetDirectories(Directory.GetCurrentDirectory()))
                if (directory.EndsWith("_Data", StringComparison.CurrentCulture))
                    return Path.Combine(directory, "Managed\\");
            ConsoleHelper.WriteError("Cannot find Managed folder. Please make sure that you've put the .exe in the root folder of the game. Press any key to exit the program.");
            return null;
        }
        public static void CreateFiles()
        {
            if (!Directory.Exists(Program.pluginsFolder))
                Directory.CreateDirectory(Program.pluginsFolder);

        }
		public static Type[] GetTypesInNamespace(Assembly assembly, string nameSpace)
		{
			return
			  assembly.GetTypes()
					  .Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal))
					  .ToArray();
		}
		public static int InjectAllHooks(Dictionary<string, List<AttributeData>> attributes, AssemblyDefinition assemblyDefinition)
		{
			// Improve
			var injectedCorrectly = 0;
			injectedCorrectly += HookAttributes.InjectHooks(attributes, assemblyDefinition);
			injectedCorrectly += AddMethodAttribute.InjectHooks(attributes, assemblyDefinition);
			return injectedCorrectly;
		}
	}
}
