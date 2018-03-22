using Microsoft.CSharp;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using CommandLine;
namespace HooksInjector
{
    internal class Options
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
        private const string ScriptsDir = "Scripts";
        private const string PluginsDir = "Plugins";
        private static string _managedFolder;
        public static string[] RefAssemblys;
        public string[] GArgs;

        public static void Main(string[] args) {
            CreateFiles();
            if (args != null) {
                var program = new Program() {
                    GArgs = args
                };
            }
            var parser = new ScriptsParser();
            var compiler = new ScriptsCompiler(PluginsDir, _managedFolder);
            string assemblyPath = _managedFolder + "/Assembly-CSharp.dll";
            if (!File.Exists(assemblyPath)) {
                Console.WriteLine("HooksInjector: ERROR: Could not find game assembly. Make sure you're running from the game's root dir.");
                Console.Read();
                return;
            }
            if (!File.Exists("Assembly-CSharp.dll")) {
                File.Copy(assemblyPath, "Assembly-CSharp.dll");

            }
            const string origAssembly = "Assembly-CSharp.dll";
            var resolver = new DefaultAssemblyResolver();
            resolver.AddSearchDirectory(_managedFolder);
            AssemblyDefinition gameAssembly = AssemblyDefinition.ReadAssembly(origAssembly, new ReaderParameters { AssemblyResolver = resolver });

            foreach (string dir in Directory.GetDirectories(ScriptsDir))
            {
                Console.WriteLine("Searching Dirs " + dir);
                string pluginFile = null;
                ScriptsParser.ParsedHook[] hooks = null;
                ScriptsParser.ParsedAccessModifier[] changedAccessModifiers = null;
                foreach (string proj in Directory.GetFiles(dir))
                {
                    Console.WriteLine("Searching projects " + proj);

                    hooks = null;
                    if (proj.EndsWith("proj"))
                    {
                        pluginFile = null;


                        Console.WriteLine("Operating System: " + System.Environment.OSVersion);
                        string xbuildpath = null;
                        if (System.Environment.OSVersion.ToString().Contains("Windows"))
                        {
                            xbuildpath = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Microsoft Visual Studio\\2017\\Community\\MSBuild\\15.0\\Bin\\msbuild.exe";
                            //Broke: Protocol Specific, changes name of game dir because linux :/
                            var projtext = File.ReadAllText(proj);
                            File.WriteAllText(proj, projtext.Replace("bpgameserver_Data", "BrokeProtocol_Data"));

                        }
                        else
                        {
                            xbuildpath = "msbuild";
                        }

                        var p = new Process
                        {
                            StartInfo = {

                                FileName = xbuildpath,
                                Arguments = $"/p:Configuration=Release {proj}",
                                UseShellExecute = false,
                                RedirectStandardOutput = true
                            }
                        };
                        p.Start();
                        Console.WriteLine("Started MSBuild");
                        string output = p.StandardOutput.ReadToEnd();
                        p.WaitForExit();
                        Console.WriteLine("MSBuild Output: \n");
                        Console.WriteLine(output);
                        Console.WriteLine("Finished Build");
                        Console.WriteLine(dir);
                        foreach (string plugin in Directory.GetFiles(dir + "/bin/Release/"))
                        {

                            if (File.Exists(PluginsDir + "/" + Path.GetFileName(plugin)))
                            {
                                File.Delete(PluginsDir + "/" + Path.GetFileName(plugin));
                            }
                            if (plugin.EndsWith("dll") && !plugin.Contains("UnityEngine") &&
                                !plugin.Contains("Assembly-CSharp") && !plugin.Contains("HookAttribute"))
                            {
                                File.Copy(dir + "/bin/Release/" + Path.GetFileName(plugin), PluginsDir + "/" + Path.GetFileName(plugin));
                            }
                        }
                    }

                }

                foreach (string plugin in Directory.GetFiles(PluginsDir))
                {
                    if (plugin.EndsWith("dll"))
                    {
                        pluginFile = plugin;
                        foreach (string script in Directory.GetFiles(dir))
                            if (script.EndsWith("cs"))
                            {
                                changedAccessModifiers = ScriptsParser.GetAccessModifiers(script);
                                if (changedAccessModifiers != null)
                                    foreach (ScriptsParser.ParsedAccessModifier currentChangedAccessModifier in changedAccessModifiers)
                                    {
                                        string cCam = currentChangedAccessModifier.AccessModifierField.Trim('"');
                                        string[] nameSplit = cCam.Split('.');
                                        string className = cCam.Substring(0, cCam.Substring(0, cCam.Length - 1).LastIndexOf('.'));
                                        string fieldName = nameSplit[nameSplit.Length - 1];
                                        FieldDefinition[] fields = gameAssembly.MainModule.GetType(className).Fields.ToArray();
                                        foreach (FieldDefinition cf in fields)
                                        {
                                            if (cf.Name == fieldName)
                                            {
                                                Console.WriteLine($"{cf.Name} | {cf.IsPublic}/{cf.IsPrivate} ({cf.IsFamilyOrAssembly}/{cf.IsFamilyAndAssembly}/{cf.IsFamily}) Matches with {fieldName}");
                                                cf.IsPrivate = false;
                                                cf.IsFamily = false;
                                                cf.IsFamilyAndAssembly = false;
                                                cf.IsFamilyOrAssembly = false;
                                                cf.IsPublic = true;
                                                Console.WriteLine($"{cf.Name} | {cf.IsPublic}/{cf.IsPrivate} ({cf.IsFamilyOrAssembly}/{cf.IsFamilyAndAssembly}/{cf.IsFamily}) Matches with {fieldName}");
                                            }
                                        }
                                    }
                                hooks = parser.GetHooks(script);
                                if (hooks != null)
                                {
                                    var injector = new Injector(gameAssembly, AssemblyDefinition.ReadAssembly(pluginFile, new ReaderParameters { AssemblyResolver = resolver }), pluginFile);
                                    foreach (ScriptsParser.ParsedHook finalhook in hooks)
                                        injector.InjectHook(finalhook, Path.GetFileName(script));
                                }
                            }
                    }
                }
            }
            gameAssembly.Write(assemblyPath);
            Console.WriteLine("HooksInjector: Hooks inserted sucessfully!");

            foreach (string plugin in Directory.GetFiles(PluginsDir)) {
                string pluginDest = _managedFolder + "/" + new FileInfo(plugin).Name;
                if (File.Exists(pluginDest)) {
                    File.Delete(pluginDest);
                }
                File.Copy(plugin, pluginDest);
            }
            Console.WriteLine("HooksInjector: Plugins copied sucessfully!");
        }

        private static void CreateFiles() {
            if (!Directory.Exists(ScriptsDir)) {
                Directory.CreateDirectory(ScriptsDir);
                Console.WriteLine("Scripts Directory created");
            }
            if (!Directory.Exists(PluginsDir)) {
                Directory.CreateDirectory(PluginsDir);
                Console.WriteLine("Plugins Directory created");
            }
            _managedFolder = GetManaged();

        }

        private static string GetManaged() {
            foreach (string directory in Directory.GetDirectories((Directory.GetCurrentDirectory()))) {
                if (directory.EndsWith("_Data", StringComparison.CurrentCulture)) {
                    return directory + "/Managed";
                }
            }
            Console.WriteLine("HooksInjector: ERROR: Managed folder not found. Place HooksInjector in $GameDir");
            return null;
        }

    }
}
