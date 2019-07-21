using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NetChalker;
using System.Diagnostics;
using CommandLine;

namespace UniversalUnityHooks
{
    public static class Program
    {
        public static string Version { get; } = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

        /// <summary>
        /// Stores all found attributes by type.
        /// <para>Example: Attributes[nameof(HookAttributes)] gets all the attributes found of type HookAttributes from all resources.</para>
        /// </summary>
        public static Dictionary<string, List<AttributeData>> Attributes { get; set; } = new Dictionary<string, List<AttributeData>>();

        public static string PluginsFolder { get; private set; }

        public static string TargetAssembly { get; private set; }

        public static string TargetAssemblyClean { get; private set; }

        public static string OutputFolder { get; private set; }

        public static Chalker Chalker { get; set; } = new Chalker();

        public static AssemblyHelper AssemblyHelper { get; private set; }

        public static AttributesHelper AttributesHelper { get; } = new AttributesHelper();

        public static Cecil Cecil { get; } = new Cecil();

        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed((options) => Options.Instance = options).WithNotParsed((_) => Util.Exit());
            Console.WriteLine($"Universal Unity Hooks v{Version}");
            Options.Instance.GetTargetAssembly(out var targetAssemblyName, out var targetAssemblyNameClean);
            TargetAssembly = targetAssemblyName;
            TargetAssemblyClean = targetAssemblyNameClean;
            PluginsFolder = Options.Instance.GetInputDirectory();
            OutputFolder = Options.Instance.GetOutputDirectory();
            Util.CreateFiles();
            Chalker.WriteMessage($"Target assembly file: {TargetAssembly}");
            Chalker.WriteMessage($"Target plugins & search directory: {PluginsFolder}");
            Console.WriteLine();
            var timer = new OperationTimer();
            AssemblyHelper = new AssemblyHelper(PluginsFolder);
            AssemblyHelper.FetchAndLoadAll();
            Console.WriteLine();

            var sum = Attributes.Sum(x => x.Value.Count);
            if (sum == 0)
            {
                Chalker.WriteError("No attributes have been loaded in. Press any key to exit the program.");
                Util.Exit();
            }
            Chalker.WriteSuccess($"Total of {sum} attribute(s) have been loaded in.");

            Console.WriteLine();
            Chalker.WriteWait("Loading target assembly..");
            try
            {
                if (File.Exists(TargetAssemblyClean))
                {
                    File.Delete(TargetAssembly);
                    File.Copy(TargetAssemblyClean, TargetAssembly);
                }
                else
                {
                    File.Copy(TargetAssembly, TargetAssemblyClean);
                }
            }
            catch (Exception ex)
            {
                Chalker.WriteError("Something went wrong while trying to get or copy the target assembly file.");
                Chalker.WriteError($"Message: {ex.Message}");
                Chalker.WriteError(ex.StackTrace);
                Util.Exit();
            }
            if (!Util.ReadAssembly(TargetAssembly, OutputFolder, out var assemblyDefinition))
            {
                Chalker.WriteError("Press any key to exit this program.");
                Util.Exit();
            }
            Chalker.WriteSuccess("Target assembly loaded in.");

            Console.WriteLine();
			var injectedCorrectly = Util.InjectAllHooks(Attributes, assemblyDefinition);
			Console.WriteLine();
            if (injectedCorrectly == 0)
            {
                Chalker.WriteError($"No methods have been injected correctly. Please read above for more information why some injections may have failed.");
            }
            else
            {
                Chalker.WriteSuccess($"Injected {(injectedCorrectly == sum ? $"all {sum}" : $"{injectedCorrectly}/{sum}")} methods. {(injectedCorrectly != sum ? "Please read above for more information why some injections may have failed." : "")}");
                Chalker.WriteWait("Writing changes to target assembly..");
                if (Cecil.WriteChanges(assemblyDefinition, TargetAssembly))
                    Chalker.WriteSuccess($"Changes have been written to the file {TargetAssembly}!");
            }
            Console.WriteLine();
            Chalker.WriteWait("Copying dll's to target assembly's folder..");
            try
            {
                foreach (var file in Directory.GetFiles(PluginsFolder, "*.dll"))
                {
                    Chalker.WriteWait($"Copying file {file} to target folder..");
                    var target = Path.Combine(OutputFolder, Path.GetFileName(file));
                    if (File.Exists(target))
                        File.Delete(target);
                    File.Copy(file, target);
                }
            }
            catch (Exception ex)
            {
                Chalker.WriteError("Something went wrong while trying to copy a plugin file.");
                Chalker.WriteError($"Message: {ex.Message}");
                Chalker.WriteError(ex.StackTrace);
                Util.Exit();
            }
            Chalker.WriteSuccess("All files have been copied.");
            timer.Stop();
            Console.WriteLine();
            Chalker.WriteSuccess($"Done. Operation took {timer.GetElapsedMs}ms. Press any key to exit the program.");
            Util.Exit(0);
        }
    }
}
