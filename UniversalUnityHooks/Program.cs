using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static UniversalUnityHooks.Util;

namespace UniversalUnityHooks
{
    class Program
    {
        public static string ver = "2.0.0";

        public static List<List<Attributes.ReturnData<CustomAttribute>>> hookAttributes = new List<List<Attributes.ReturnData<CustomAttribute>>>();
        public static string managedFolder;
        public static string TargetAssembly = "Assembly-CSharp.dll";
        public static string pluginsFolder = "Plugins\\";
        public static bool WaitForInput;
        static void Main(string[] args)
        {
            WaitForInput |= args.Contains("-waitforinput");
            Console.WriteLine($"Universal Unity Hooks v{ver}");
            var timer = new OperationTimer();
            Util.CreateFiles();
            managedFolder = GetManagedDirectory();
            if (managedFolder == null)
            {
                if (WaitForInput)
                    Console.ReadKey();
                Environment.Exit(-1);
            }
            TargetAssembly = Path.Combine(managedFolder, TargetAssembly);
            var TargetAssemblyClean = Path.Combine(managedFolder, TargetAssembly + ".clean");
            ConsoleHelper.WriteMessage($"Found target assembly: {TargetAssembly}");
            Console.WriteLine();

            Assemblies.GetAllAssembliesInsideDirectory(pluginsFolder);
            Console.WriteLine();

            var sum = hookAttributes.Sum(x => x.Count);
            if (sum == 0)
            {
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"No attributes have been loaded in. Press any key to exit the program.");
                if (WaitForInput)
                    Console.ReadKey();
                Environment.Exit(-1);
            }
            ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, $"Total of {sum} attribute(s) have been loaded in.");

            Console.WriteLine();
            ConsoleHelper.WriteMessage($"Loading target assembly..");
            try
            {
                if (File.Exists(TargetAssemblyClean))
                {
                    File.Delete(TargetAssembly);
                    File.Copy(TargetAssemblyClean, TargetAssembly);
                }
                else
                    File.Copy(TargetAssembly, TargetAssemblyClean);
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, "Something went wrong while trying to get or copy the target assembly file.");
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"Message: {ex.Message}");
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, ex.StackTrace);
                if (WaitForInput)
                    Console.ReadKey();
                Environment.Exit(-1);
            }
            var assemblyDefinition = Cecil.ReadAssembly(TargetAssembly);
            if (assemblyDefinition == null)
            {
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, "Press any key to exit this program.");
                if (WaitForInput)
                    Console.ReadKey();
                Environment.Exit(-1);
            }
            ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, $"Target assembly loaded in.");

            Console.WriteLine();
            var injectedCorrectly = 0;
            foreach (var _h in hookAttributes)
            {
                foreach (var hook in _h)
                {
                    var returnData = Cecil.ConvertStringToClassAndMethod(hook.attribute.ConstructorArguments[0].Value.ToString(), assemblyDefinition);
                    if (returnData == null)
                        continue;
                    if (Cecil.Inject(returnData, hook, assemblyDefinition, hook.assembly))
                        ++injectedCorrectly;
                }
            }

            Console.WriteLine();
            if (injectedCorrectly == 0)
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"No methods have been injected correctly. Please read above for more information why some injections may have failed.");
            else
            {
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, $"Injected {(injectedCorrectly == sum ? $"all {sum}" : $"{injectedCorrectly}/{sum}")} methods. {(injectedCorrectly != sum ? "Please read above for more information why some injections may have failed." : "")}");
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Wait, "Wrting changes to target assembly..");
                if (Cecil.WriteChanges(assemblyDefinition))
                    ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, $"Changes have been written to the file {TargetAssembly}!");
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
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, "Something went wrong while trying to copy a plugin file.");
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"Message: {ex.Message}");
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, ex.StackTrace);
                if (WaitForInput)
                    Console.ReadKey();
                Environment.Exit(-1);
            }
            ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, "All files have been copied.");
            timer.Stop();
            Console.WriteLine();
            ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, $"Done. Operation took {timer.GetElapsedMs}ms. Press any key to exit the program.");
            if (WaitForInput)
                Console.ReadKey();
            Environment.Exit(0);
        }
    }
}
