using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
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
        public List<DirectoryInfo> ResolveDirectories { get; set; } = new List<DirectoryInfo>();

        [CommandOption("copyinput", 'c', Description = "Should the program copy the input files to the target dll folder?")]
        public bool CopyToTarget { get; set; } = true;

        [CommandOption("dry", 'd', Description = "If this value is true, the program will not write any changes to the target dll. This however will replace the dll with the clear one (filename.dll.clean), if found.")]
        public bool DryRun { get; set; }

        [CommandOption("addtargetresolver", 'm', Description = "Automatically adds the target parent folder to the assembly resolver.")]
        public bool AddTargetDirectoryResolve { get; set; } = true;

        [CommandOption("verbosity", 'v', Description = "The verbosity level currently applied. Possible values: 0 (nothing)\n                    1 - Input file information\n                    2 - Step by step logs\n                    3 - Minor extra verbosity\n                    4 - I/O Access (Reading/writing files & moving files)")]
        public uint Verbosity { get; set; } = 0;

        private readonly ILogger _logger = new Logger<Commands.ExecuteModules>();

        private readonly Stopwatch _sw = new Stopwatch();

        public ValueTask ExecuteAsync(IConsole _)
        {
            _logger.Settings.DebugVerbosity = (int)Verbosity;
            _sw.Start();
            _logger.LogInformation($"UniversalUnityHooks v{Program.Version}");
            _logger.LogDebug($"System Version: {Environment.OSVersion}", 3);
            if (Target == null)
            {
                _logger.LogWarning("Target is null, defaulting to '?_Data/Managed/Assembly-CSharp.dll'.");
                Target = Util.FindAssemblyCSharp(Directory.GetCurrentDirectory());
            }
            _logger.LogDebug($"Input: '{string.Join(",", Files)}'\nTarget: '{Target}'", 3);
            // TODO: More asserts, especially on input files
            CliAssert.IsRequired(Target, "Target Assembly (target,t)");
            CliAssert.IsFile(Target);
            CliAssert.HasExtension(Target, ".dll");
            _logger.LogDebug("Asserts passed, adding resolver...", 2);
            if (AddTargetDirectoryResolve)
            {
                ResolveDirectories.Add(Target.Directory);
            }
            var resolver = Util.CreateAssemblyResolver(ResolveDirectories);
            if (File.Exists(Target.FullName + ".clean"))
            {
                _logger.LogDebug($"IO: '.clean' File exists, overwriting target assembly with clean file...", 4);
                File.Delete(Target.FullName);
                File.Copy(Target.FullName + ".clean", Target.FullName, true);
            }
            _logger.LogDebug($"IO: Reading assembly from '{Target.FullName}'...", 4);
            var targetDefinition = AssemblyDefinition.ReadAssembly(Target.FullName, new ReaderParameters { AssemblyResolver = resolver, InMemory = true, ReadWrite = false });
            var modules = new List<IModule>();
            modules.Add(new HookModule());
            modules.Add(new AddMethodModule());
            modules.Add(new ILProcessorModule());
            modules.Add(new Modules.FluentInjectorModule());
            _logger.LogDebug($"{modules.Count} Module(s) loaded.", 2);

            Files = Util.FlattenDirectory(Files, "*.dll");

            foreach (var input in Files)
            {
                ReadAndExecute(input, modules, targetDefinition);
            }
            _logger.NewLine();
            if (!DryRun)
            {
                WriteChanges(targetDefinition);
            }

            if (CopyToTarget)
            {
                CopyInputFiles();
            }
            _logger.LogInformation($"Operation completed. Operation took {_sw.ElapsedMilliseconds}ms.");
            return default;
        }

        private void LogAssemblyInfo(ILogger logger, FileInfo file, Assembly assembly)
        {
            var name = assembly.GetName();
            var sb = new StringBuilder();
            sb.Append("File Name: ".PadRight(15)).AppendLine(file.Name);
            sb.Append("Name: ".PadRight(15)).AppendLine(name.ToString());
            sb.Append("ProcessorArch: ".PadRight(15)).AppendLine(name.ProcessorArchitecture.ToString());
            sb.Append("CLR Version: ".PadRight(15)).AppendLine(assembly.ImageRuntimeVersion);
            logger.LogDebug(sb.ToString());
        }

        private void ReadAndExecute(FileInfo input, List<IModule> modules, AssemblyDefinition targetDefinition)
        {
            _logger.NewLine();
            _logger.LogDebug($"IO: Reading input file '{input.FullName}'...", 4);
            var assemblyDefinition = AssemblyDefinition.ReadAssembly(input.FullName);
            var inputReflection = Assembly.LoadFrom(input.FullName);
            var name = inputReflection.GetName();
            var inputLogger = new Logger($"Input::{name.Name}@{name.Version}", _logger.Settings);
            LogAssemblyInfo(inputLogger, input, inputReflection);
            var st = new Stopwatch();

            foreach (var type in assemblyDefinition.MainModule.GetTypes())
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
                            _logger.NewLine();
                            inputLogger.LogInformation($"Found attribute '{module.GetType().Name}<{customAttribute.AttributeType.Name}>' attached to method '{type.FullName}.{method.Name}'.");
                            var methodInfo = inputReflection.GetType(type.FullName, true).GetMethod(method.Name);
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
            _logger.LogDebug("IO: Writing changes to target assembly...", 4);
            File.Copy(Target.FullName, Target.FullName + ".clean", true);
            File.Delete(Target.FullName);
            targetDefinition.Write(Target.FullName);
            _logger.LogInformation("Changes written to target assembly.");
        }

        private void CopyInputFiles()
        {
            _logger.LogDebug("Copying files to target...", 4);
            foreach (var input in Files)
            {
                var to = Path.Combine(Target.DirectoryName, input.Name);
                _logger.LogDebug($"IO: {input.Name} -> {to}", 4);
                input.CopyTo(to, true);
            }
            _logger.LogInformation("Files copied to target.");
        }
    }
}