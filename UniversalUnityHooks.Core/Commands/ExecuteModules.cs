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

namespace UniversalUnityHooks.Core.Commands
{
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

        public ValueTask ExecuteAsync(IConsole console)
        {
            var totalSw = new Stopwatch();
            totalSw.Start();
            var logger = new Logger("Core");
            // TODO: Fetch version number from csproj
            logger.LogInformation("UniversalUnityHooks 3.0");
            // foreach (var item in Enum.GetValues(typeof(Logging.Models.LogLevel)))
            // {
            //     logger.Log((Logging.Models.LogLevel)item, "Hi there");
            // }
            if (Target == null)
            {
                logger.LogInformation("Target is null, defaulting to '?_Data/Managed/Assembly-CSharp.dll'.");
                Target = Util.FindAssemblyCSharp(Directory.GetCurrentDirectory());
            }
            logger.LogDebug($"Input: '{string.Join(",", Files)}'\nTarget: '{Target}'");
            // TODO: More asserts, especially on input files
            CliAssert.IsRequired(Target, "Target Assembly (target,t)");
            CliAssert.IsNotDirectory(Target);
            CliAssert.HasExtension(Target, ".dll");
            logger.LogDebug("Asserts passed, adding resolver...");
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
                logger.LogDebug($"'.clean' File exists, overwriting target assembly with clean file...");
                File.Delete(Target.FullName);
                File.Copy(Target.FullName + ".clean", Target.FullName, true);
            }
            logger.LogDebug($"Reading assembly from '{Target.FullName}'...");
            var targetDefinition = AssemblyDefinition.ReadAssembly(Target.FullName);
            var modules = new List<IModule>();
            modules.Add(new HookModule());
            modules.Add(new AddMethodModule());
            modules.Add(new ILProcessorModule());
            modules.Add(new Modules.LowLevelModule());
            logger.LogDebug($"{modules.Count} Module(s) loaded.");
            foreach (var input in Files)
            {
                logger.LogDebug($"Reading input file '{input.FullName}'...");
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
                                // console.Output.WriteLine($"{method.FullName} - Executing attribute\n - Attribute found: {customAttribute.AttributeType.Name}\n - Module handler: {module.GetType().Name}");
                                var methodInfo = inputReflection.GetType(type.FullName).GetMethod(method.Name);
                                st.Start();
                                module.Execute(method, methodInfo, type, assemblyDefinition, targetDefinition);
                                inputLogger.LogInformation($"Module executed in {st.ElapsedMilliseconds}ms.");
                            }
                        }
                    }
                }
            }
            Console.WriteLine();
            if (!DryRun)
            {
                logger.LogInformation("Writing changes to target assembly...");
                File.Copy(Target.FullName, Target.FullName + ".clean", true);
                File.Delete(Target.FullName);
                targetDefinition.Write(Target.FullName);
                logger.LogInformation("Changes written!");
            }


            // Copy hooks
            if (CopyToTarget)
            {
                logger.LogInformation("Copying files to target...");
                foreach (var input in Files)
                {
                    var to = Path.Combine(Target.DirectoryName, input.Name);
                    logger.LogDebug($"{input.Name} -> {to}");
                    input.CopyTo(to, true);
                }
            }
            logger.LogInformation($"Operation completed. Operation took {totalSw.ElapsedMilliseconds}ms.");
            return default;
        }
    }
}