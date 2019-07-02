using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UniversalUnityHooks.Attributes;

namespace UniversalUnityHooks
{
    public static class Util
    {
        public static bool GetManagedDirectory(out string directory)
        {
            foreach (string currDirectory in Directory.GetDirectories(Directory.GetCurrentDirectory()))
            {
                if (currDirectory.EndsWith("_Data", StringComparison.CurrentCulture))
                {
                    directory = Path.Combine(currDirectory, "Managed");
                    return true;
                }
            }
            Program.Chalker.WriteError("Cannot find Managed folder. Please make sure that you've put the .exe in the root folder of the game. Press any key to exit the program.");
            directory = null;
            return false;
        }

        public static void CreateFiles()
        {
            if (!Directory.Exists(Program.PluginsFolder))
                Directory.CreateDirectory(Program.PluginsFolder);
        }

		public static IEnumerable<Type> GetTypesInNamespace(Assembly assembly, string nameSpace)
		{
			return assembly.GetTypes().Where(t => string.Equals(t.Namespace, nameSpace, StringComparison.Ordinal));
		}

		public static int InjectAllHooks(Dictionary<string, List<AttributeData>> attributes, AssemblyDefinition assemblyDefinition)
		{
			// Improve
			var injectedCorrectly = 0;
			injectedCorrectly += HookAttributes.InjectHooks(attributes, assemblyDefinition);
			injectedCorrectly += Attributes.AddMethodAttribute.InjectHooks(attributes, assemblyDefinition);
			return injectedCorrectly;
		}

        public static void Exit(int exitCode = -1)
        {
            if (Options.Instance?.WaitForInput == true)
                Console.ReadKey();
            Environment.Exit(exitCode);
        }

        public static bool ReadAssembly(string assemblyPath, string searchDirectory, out AssemblyDefinition assembly)
        {
            try
            {
                var resolver = new DefaultAssemblyResolver();
                resolver.AddSearchDirectory(searchDirectory);
                assembly = AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters { AssemblyResolver = resolver });
                return true;
            }
            catch (Exception ex)
            {
                Program.Chalker.WriteError($"Cannot load in the assembly {assemblyPath} (Cecil).");
                Program.Chalker.WriteError($"Message: {ex.Message}");
                Program.Chalker.WriteError(ex.StackTrace);
                assembly = null;
                return false;
            }
        }
    }
}
