using Microsoft.CSharp;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using CommandLine;
using Microsoft.Build.Framework;
namespace HooksInjector
{
    class Options
    {
        [Option('r', "ref", Required = false, HelpText = "Adds external assembly references, from the Managed folder, GAC, or specified path.")]
        public string[] Refs { get; set; }

        [Option('o', "nooptimize", HelpText = "Turns off Compiler Optimization, this can be useful for debugging.")]
        public bool Optimize { get; set; }

        [ParserState]
        public IParserState LastParserState { get; set; }
    }

    public class Program
    {
        private const string scriptsDir = "Scripts";
        private const string pluginsDir = "Plugins";
        private static string managedFolder;
        public static string[] refAssemblys;
        public string[] gArgs;

        public static void Main(string[] args) {
            CreateFiles();
            if (args != null) {
                var program = new Program() {
                    gArgs = args
                };
            }
            var parser = new ScriptsParser();
            var compiler = new ScriptsCompiler(pluginsDir, managedFolder);
            var assemblyPath = managedFolder + "/Assembly-CSharp.dll";
            if (!File.Exists(assemblyPath)) {
                Console.WriteLine("HooksInjector: ERROR: Could not find game assembly. Make sure you're running from the game's root dir.");
                Console.Read();
                return;
            }
            if (!File.Exists("Assembly-CSharp.dll")) {
                File.Copy(assemblyPath, "Assembly-CSharp.dll");

            }
            const string origAssembly = "Assembly-CSharp.dll";
            var gameAssembly = AssemblyDefinition.ReadAssembly(origAssembly);

            foreach (var dir in Directory.GetDirectories(scriptsDir)) {
                foreach (var proj in Directory.GetFiles(dir)) {
                    ScriptsParser.ParsedHook[] hooks = null;
                    string pluginFile = null;
                    if (proj.EndsWith(".proj")) {
                        

                        
                        var p = new Process {
                            StartInfo = {
                                FileName = "xbuild.exe",
                                Arguments = $"/p:Configuration=Release {proj}",
                                UseShellExecute = false,
                                RedirectStandardOutput = true
                            }
                        };
                        p.Start();

                        var output = p.StandardOutput.ReadToEnd();
                        p.WaitForExit();
                        Console.WriteLine("XBuild Output: \n");
                        Console.WriteLine(output);
                        pluginFile = proj.Replace("csproj", "dll");
                    }
                    else if (proj.EndsWith("cs")) {
                            hooks = parser.GetHooks(proj);
                        
                    }
                    if (hooks != null && pluginFile != null)
                    {
                        var injector = new Injector(gameAssembly, AssemblyDefinition.ReadAssembly(pluginFile), pluginFile);
                        foreach (var hook in hooks) {
                            injector.InjectHook(hook);
                        }
                    }
                }
                
                
            }
            /*
            
             OLD CODE INCASE THIS IS BROKEN
            foreach (var scriptfile in Directory.GetFiles(scriptsDir)) {
                var hooks = parser.GetHooks(scriptfile);
                var pluginFile = compiler.CompileScript(scriptfile);

                if (!File.Exists(pluginFile)) {
                    Console.WriteLine("HooksInjector: ERROR: " + pluginFile + " Was not compiled sucessfully.");
                    Console.ReadLine();
                    return;

                }

                var injector = new Injector(gameAssembly, AssemblyDefinition.ReadAssembly(pluginFile), pluginFile);
                foreach (var hook in hooks) {
                    injector.InjectHook(hook);
                }
            }
            */
            gameAssembly.Write(assemblyPath);
            Console.WriteLine("HooksInjector: Hooks inserted sucessfully!");

            foreach (var plugin in Directory.GetFiles(pluginsDir)) {
                var pluginDest = managedFolder + "/" + new FileInfo(plugin).Name;
                if (File.Exists(pluginDest)) {
                    File.Delete(pluginDest);
                }
                File.Copy(plugin, pluginDest);
            }
            Console.WriteLine("HooksInjector: Plugins copied sucessfully!");
        }

        private static void CreateFiles() {
            if (!Directory.Exists(scriptsDir)) {
                Directory.CreateDirectory(scriptsDir);
                Console.WriteLine("Scripts Directory created");
            }
            if (!Directory.Exists(pluginsDir)) {
                Directory.CreateDirectory(pluginsDir);
                Console.WriteLine("Plugins Directory created");
            }
            managedFolder = GetManaged();

        }

        private static string GetManaged() {
            foreach (var directory in Directory.GetDirectories((Directory.GetCurrentDirectory()))) {
                if (directory.EndsWith("_Data", StringComparison.CurrentCulture)) {
                    return directory + "/Managed";
                }
            }
            Console.WriteLine("HooksInjector: ERROR: Managed folder not found. Place HooksInjector in $GameDir");
            return null;
        }

    }
}
