using CommandLine;
using System;
using System.IO;

namespace UniversalUnityHooks
{
    public class Options
    {
        public static Options Instance { get; internal set; }

        [Option('t', "target", Required = false, HelpText = "Set the target assembly file to be injected.")]
        public string TargetAssembly { get; set; }

        [Option('i', "input", Required = false, HelpText = "Set the input assemblies to be injected.")]
        public string InputDirectory { get; set; }

        [Option('o', "output", Required = false, HelpText = "Set the output directory for assemblies to be copied to. NOTE: This will also be the search directory. Recommended to keep default.")]
        public string OutputDirectory { get; set; }

        [Option('w', "wait", Required = false, HelpText = "Waits for input before continuing.")]
        public bool WaitForInput { get; set; }

        public bool GetTargetAssembly(out string fileName, out string fileNameClean)
        {
            if (!string.IsNullOrWhiteSpace(TargetAssembly))
            {
                fileName = TargetAssembly;
                fileNameClean = fileName + ".clean";
                return true;
            }
            if (!Util.GetManagedDirectory(out var managedFolder))
            {
                Util.Exit();
                fileName = null;
                fileNameClean = null;
                return false;
            }
            fileName = Path.Combine(managedFolder, "Assembly-CSharp.dll");
            fileNameClean = fileName + ".clean";
            return true;
        }

        public string GetInputDirectory()
        {
            return string.IsNullOrWhiteSpace(InputDirectory) ? Path.Combine(Environment.CurrentDirectory, "Plugins") : InputDirectory;
        }

        public string GetOutputDirectory()
        {
            if (!string.IsNullOrWhiteSpace(OutputDirectory))
                return OutputDirectory;
            if (!Util.GetManagedDirectory(out var managedFolder))
            {
                Util.Exit();
                return null;
            }
            return managedFolder;
        }
    }
}
