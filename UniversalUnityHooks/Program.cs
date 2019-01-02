using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UniversalUnityHooks.Attributes;
using static UniversalUnityHooks.Util;

namespace UniversalUnityHooks
{
    class Program
    {
        public static string ver = "2.1.0";

		// Improve
        public static List<List<AttributesHelper.ReturnData<CustomAttribute>>> HookAttributes { get; set; } = new List<List<AttributesHelper.ReturnData<CustomAttribute>>>();
		public static List<List<AttributesHelper.ReturnData<CustomAttribute>>> AddMethodAttributes { get; set; } = new List<List<AttributesHelper.ReturnData<CustomAttribute>>>();


		public static string managedFolder;
        public static string targetAssembly = "Assembly-CSharp.dll";
        public static string pluginsFolder = "Plugins\\";
        public static bool waitForInput;
        static void Main(string[] args)
        {
            waitForInput |= args.Contains("-waitforinput");
            Console.WriteLine($"Universal Unity Hooks v{ver}");
            var timer = new OperationTimer();
            Util.CreateFiles();
            managedFolder = GetManagedDirectory();
            if (managedFolder == null)
            {
                if (waitForInput)
                    Console.ReadKey();
                Environment.Exit(-1);
            }
            targetAssembly = Path.Combine(managedFolder, targetAssembly);
            var TargetAssemblyClean = Path.Combine(managedFolder, targetAssembly + ".clean");
            ConsoleHelper.WriteMessage($"Found target assembly: {targetAssembly}");
            Console.WriteLine();

            Assemblies.GetAllAssembliesInsideDirectory(pluginsFolder);
            Console.WriteLine();

			// Improve
            var sum = HookAttributes.Sum(x => x.Count) + AddMethodAttributes.Sum(x=>x.Count);
            if (sum == 0)
            {
                ConsoleHelper.WriteError($"No attributes have been loaded in. Press any key to exit the program.");
                if (waitForInput)
                    Console.ReadKey();
                Environment.Exit(-1);
            }
            ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, $"Total of {sum} attribute(s) have been loaded in.");

            Console.WriteLine();
            ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Wait, $"Loading target assembly..");
            try
            {
                if (File.Exists(TargetAssemblyClean))
                {
                    File.Delete(targetAssembly);
                    File.Copy(TargetAssemblyClean, targetAssembly);
                }
                else
                    File.Copy(targetAssembly, TargetAssemblyClean);
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Something went wrong while trying to get or copy the target assembly file.");
                ConsoleHelper.WriteError($"Message: {ex.Message}");
                ConsoleHelper.WriteError(ex.StackTrace);
                if (waitForInput)
                    Console.ReadKey();
                Environment.Exit(-1);
            }
            var assemblyDefinition = Cecil.ReadAssembly(targetAssembly);
            if (assemblyDefinition == null)
            {
                ConsoleHelper.WriteError("Press any key to exit this program.");
                if (waitForInput)
                    Console.ReadKey();
                Environment.Exit(-1);
            }
            ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, $"Target assembly loaded in.");

            Console.WriteLine();
            var injectedCorrectly = 0;
            foreach (var resourceAttributes in HookAttributes)
            {
                foreach (var hook in resourceAttributes)
                {
					// Improve: Add check for multiple args here
					var returnData = Cecil.ConvertStringToClassAndMethod(hook.Attribute.ConstructorArguments[0].Value.ToString(), assemblyDefinition);
                    if (returnData == null)
                        continue;
                    if (Cecil.Inject(returnData, hook))
                        ++injectedCorrectly;
                }
            }
			foreach (var resourceAttributes in AddMethodAttributes)
			{
				foreach (var addMethod in resourceAttributes)
				{
					// Improve: Add check for multiple args here
					var typeDefinition = Cecil.ConvertStringToClass(addMethod.Attribute.ConstructorArguments[0].Value.ToString(), assemblyDefinition);
					var methodName = addMethod.Attribute.ConstructorArguments[1].Value.ToString();
					if (Cecil.MethodExists(typeDefinition, methodName))
					{
						ConsoleHelper.WriteError($"Method \"{methodName}\" Already exists in type {typeDefinition.Name}.");
						continue;
					}
					if (Cecil.InjectNewMethod(typeDefinition, assemblyDefinition, methodName, addMethod))
						++injectedCorrectly;
				}
			}
			Console.WriteLine();
            if (injectedCorrectly == 0)
                ConsoleHelper.WriteError($"No methods have been injected correctly. Please read above for more information why some injections may have failed.");
            else
            {
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, $"Injected {(injectedCorrectly == sum ? $"all {sum}" : $"{injectedCorrectly}/{sum}")} methods. {(injectedCorrectly != sum ? "Please read above for more information why some injections may have failed." : "")}");
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Wait, "Writing changes to target assembly..");
                if (Cecil.WriteChanges(assemblyDefinition))
                    ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, $"Changes have been written to the file {targetAssembly}!");
            }
            Console.WriteLine();
            ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Wait, "Copying dll's to target assembly's folder..");
            try
            {
                foreach (var file in Directory.GetFiles(pluginsFolder, "*.dll"))
                {
                    ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Wait, $"Copying file {file} to target folder..");
                    var target = Path.Combine(managedFolder, Path.GetFileName(file));
                    if (File.Exists(target))
                        File.Delete(target);
                    File.Copy(file, target);
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Something went wrong while trying to copy a plugin file.");
                ConsoleHelper.WriteError($"Message: {ex.Message}");
                ConsoleHelper.WriteError(ex.StackTrace);
                if (waitForInput)
                    Console.ReadKey();
                Environment.Exit(-1);
            }
            ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, "All files have been copied.");
            timer.Stop();
            Console.WriteLine();
            ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, $"Done. Operation took {timer.GetElapsedMs}ms. Press any key to exit the program.");
            if (waitForInput)
                Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
