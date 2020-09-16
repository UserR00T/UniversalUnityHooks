using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using Mono.Cecil;
using UniversalUnityHooks.Core.Interfaces;
using UniversalUnityHooks.Core.Modules;
using UniversalUnityHooks.Core.Utility;
using UniversalUnityHooks.Logging;
using UniversalUnityHooks.Logging.Interfaces;

namespace UniversalUnityHooks.Core.Commands
{
    /// <summary>
    /// Loads in input and target assembly files and executes modules on each attribute-decorated method inside the input assembly files.
    /// </summary>
    [Command("execute", Description = "Loads in input and target assembly files and executes modules on each attribute-decorated method inside the input assembly files.")]
    public class ExecuteModules : ICommand
    {
        [CommandOption("input", 'i', Description = "Input files.", IsRequired = true)]
        public List<FileInfo> Files { get; set; }

        [CommandOption("target", 't', Description = "Target assembly. This is the assembly that will be injected.")]
        public FileInfo Target { get; set; }

        [CommandOption("resolve", 'r', Description = "Extra references shall be resolved from these folders. You most likely want to also reference the TargetAssembly folder as most references will be located in that folder.")]
        public List<FileInfo> ResolveDirectories { get; set; } = new List<FileInfo>();

        [CommandOption("copyinput", 'c', Description = "Should the program copy the input files to the target dll folder?")]
        public bool CopyToTarget { get; set; } = true;

        [CommandOption("dry", 'd', Description = "If this value is true, the program will not write any changes to the target dll. This however will replace the dll with the clear one (filename.dll.clean), if found.")]
        public bool DryRun { get; set; }

        [CommandOption("addtargetresolver", 'm', Description = "Automatically adds the target parent folder to the assembly resolver.")]
        public bool AddTargetDirectoryResolve { get; set; } = true;

        private readonly ILogger _logger = new Logger("Core");

        public ValueTask ExecuteAsync(IConsole _)
        {
            var totalSw = new Stopwatch();
            totalSw.Start();
            // TODO: Fetch version number from csproj
            _logger.LogInformation("UniversalUnityHooks 3.0");
            if (Target == null)
            {
                _logger.LogInformation("Target is null, defaulting to '?_Data/Managed/Assembly-CSharp.dll'.");
                Target = Util.FindAssemblyCSharp(Directory.GetCurrentDirectory());
            }
            _logger.LogDebug($"Input: '{string.Join(",", Files)}'\nTarget: '{Target}'");
            // TODO: More asserts, especially on input files
            CliAssert.IsRequired(Target, "Target Assembly (target,t)");
            CliAssert.IsNotDirectory(Target);
            CliAssert.HasExtension(Target, ".dll");
            _logger.LogDebug("Asserts passed, adding resolver...");
            var resolver = new DefaultAssemblyResolver();
            foreach (var resolveDirectory in ResolveDirectories)
            {
                resolver.AddSearchDirectory(resolveDirectory.FullName);
            }
            if (AddTargetDirectoryResolve)
            {
                resolver.AddSearchDirectory(Target.DirectoryName);
            }
            if (File.Exists(Target.FullName + ".clean"))
            {
                _logger.LogDebug($"'.clean' File exists, overwriting target assembly with clean file...");
                File.Delete(Target.FullName);
                File.Copy(Target.FullName + ".clean", Target.FullName, true);
            }
            _logger.LogDebug($"Reading assembly from '{Target.FullName}'...");
            var targetDefinition = AssemblyDefinition.ReadAssembly(Target.FullName, new ReaderParameters { AssemblyResolver = resolver });
            var modules = new List<IModule>();
            modules.Add(new HookModule());
            modules.Add(new AddMethodModule());
            modules.Add(new ILProcessorModule());
            modules.Add(new Modules.LowLevelModule());
            _logger.LogDebug($"{modules.Count} Module(s) loaded.");

            // Loops over input files and appends all files inside directories to files
            for (int i = 0; i < Files.Count; i++)
            {
                var file = Files[i];
                var attr = File.GetAttributes(file.FullName);
                if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
                {
                    continue;
                }
                _logger.LogDebug("Input directory: '{file.FullName}' - Adding files from directory");
                // Adds all files from directory to list
                Files.AddRange(new DirectoryInfo(file.FullName).GetFiles("*.dll"));
                Files.RemoveAt(i);
            }

            foreach (var input in Files)
            {
                ReadAndExecute(input, modules, targetDefinition);
            }
            Console.WriteLine();
            if (!DryRun)
            {
                WriteChanges(targetDefinition);
            }

            if (CopyToTarget)
            {
                CopyInputFiles();
            }
            _logger.LogInformation($"Operation completed. Operation took {totalSw.ElapsedMilliseconds}ms.");
            return default;
        }

        private void ReadAndExecute(FileInfo input, List<IModule> modules, AssemblyDefinition targetDefinition)
        {
            Console.WriteLine();
            _logger.LogDebug($"Reading input file '{input.FullName}'...");
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(input.FullName);
            var inputReflection = Assembly.LoadFrom(input.FullName);
            var types = assemblyDefinition.MainModule.GetTypes().ToList();
            var name = inputReflection.GetName();
            var inputLogger = new Logger($"Input::{name.Name}@{name.Version}");
            var sb = new StringBuilder();
            sb.Append("File Name: ".PadRight(16)).AppendLine(input.Name);
            sb.Append("Name: ".PadRight(16)).AppendLine(name.ToString());
            sb.Append("ProcessorArch: ".PadRight(16)).AppendLine(name.ProcessorArchitecture.ToString());
            sb.Append("CLR Version: ".PadRight(16)).AppendLine(inputReflection.ImageRuntimeVersion);
            sb.Append("Detected Types: ".PadRight(16)).Append(types.Count.ToString());
            inputLogger.LogDebug(sb.ToString());
            var st = new Stopwatch();
            
            foreach (var type in types)
            {
                foreach (var method in type.Methods)
                {
                    var customAttributes = method.CustomAttributes;
                    if (customAttributes?.Count == 0)
                    {
                        continue;
                    }
                    foreach (var customAttribute in customAttributes)
                    {
                        foreach (var module in modules)
                        {
                            if (!module.IsValidAttribute(customAttribute))
                            {
                                continue;
                            }
                            st.Reset();
                            Console.WriteLine();
                            inputLogger.LogInformation($"Found attribute '{module.GetType().Name}<{customAttribute.AttributeType.Name}>' attached to method '{type.FullName}.{method.Name}'.");
                            var methodInfo = inputReflection.GetType(type.FullName).GetMethod(method.Name);
                            st.Start();
                            module.Execute(method, methodInfo, type, assemblyDefinition, targetDefinition);
                            inputLogger.LogInformation($"Module executed in {st.ElapsedMilliseconds}ms.");
                        }
                    }
                }
            }
        }

        private void WriteChanges(AssemblyDefinition targetDefinition)
        {
            _logger.LogInformation("Writing changes to target assembly...");
            File.Copy(Target.FullName, Target.FullName + ".clean", true);
            File.Delete(Target.FullName);
            targetDefinition.Write(Target.FullName);
            _logger.LogInformation("Changes written!");
        }

        private void CopyInputFiles()
        {
            _logger.LogInformation("Copying files to target...");
            foreach (var input in Files)
            {
                var to = Path.Combine(Target.DirectoryName, input.Name);
                _logger.LogDebug($"{input.Name} -> {to}");
                input.CopyTo(to, true);
            }
        }
    }
}