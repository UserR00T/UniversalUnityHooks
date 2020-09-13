using CliFx;
using CliFx.Attributes;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Mono.Cecil;
using System.Reflection;
using UniversalUnityHooks.Core.Modules;
using UniversalUnityHooks.Core.Interfaces;
using UniversalUnityHooks.Core.Utility;

namespace UniversalUnityHooks.Core
{
    [Command]
    public class MainCommand : ICommand
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

        public ValueTask ExecuteAsync(IConsole console)
        {
            console.Output.WriteLine($"input command {string.Join(",", Files)}");
            if (Target == null)
            {
                console.Output.WriteLine("Target is null, defaulting to '?_Data/Managed/Assembly-CSharp.dll'.");
                Target = Util.FindAssemblyCSharp(Directory.GetCurrentDirectory());
            }
            // TODO: More asserts, especially on input files
            CliAssert.IsRequired(Target, "Target Assembly (target,t)");
            CliAssert.IsNotDirectory(Target);
            CliAssert.HasExtension(Target, ".dll");
            var resolver = new DefaultAssemblyResolver();
            foreach (var resolveDirectory in ResolveDirectories)
            {
                resolver.AddSearchDirectory(resolveDirectory.FullName);
            }
            var targetDefinition = AssemblyDefinition.ReadAssembly(Target.FullName);

            if (File.Exists(Target.FullName + ".clean"))
            {
                File.Copy(Target.FullName + ".clean", Target.FullName, true);
            }

            var modules = new List<IModule>();
            modules.Add(new HookModule());
            modules.Add(new AddMethodModule());
            modules.Add(new ILProcessorModule());
            modules.Add(new Modules.LowLevelModule());
            foreach (var input in Files)
            {
                console.Output.WriteLine($"Input file: {input.FullName}");
                var assemblyDefinition = AssemblyDefinition.ReadAssembly(input.FullName);
                var inputReflection = Assembly.LoadFrom(input.FullName);
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
                                console.Output.WriteLine($"{method.FullName} - Executing attribute\n - Attribute found: {customAttribute.AttributeType.Name}\n - Module handler: {module.GetType().Name}");
                                var methodInfo = inputReflection.GetType(type.FullName).GetMethod(method.Name);
                                module.Execute(method, methodInfo, type, assemblyDefinition, targetDefinition);
                            }
                        }
                    }
                }
            }
            if (!DryRun)
            {
                File.Copy(Target.FullName, Target.FullName + ".clean", true);
                File.Delete(Target.FullName);
                targetDefinition.Write(Target.FullName);
            }


            // Copy hooks
            if (CopyToTarget)
            {
                foreach (var input in Files)
                {
                    input.CopyTo(Path.Combine(Target.DirectoryName, input.Name) , true);
                }
            }

            return default;
        }
    }

    public static class Program
    {
        public static async Task<int> Main() =>
            await new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                .Build()
                .RunAsync();
    }
}
