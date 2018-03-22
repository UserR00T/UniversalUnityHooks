using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;

namespace HooksInjector
{
    public class ScriptsCompiler
    {
        private string _pluginsDir;
        private string _managedFolder;
        public ScriptsCompiler(string plugins, string managed) {
            _pluginsDir = plugins;
            _managedFolder = managed;
            if (!Directory.Exists(_pluginsDir)) {
                Console.WriteLine("HooksInjector: ERROR: Plugins Directory does not exist! Check you have permission to create directories here.");
                Console.Read();
                return;

            }
        }

        public string CompileScript(string scriptFile) {
            var options = new Options();
            var main = new Program();
            var provider = new CSharpCodeProvider();
            string output = "Plugins/" + new FileInfo(scriptFile).Name.Replace(".cs", ".dll");
            var cp = new CompilerParameters {
                GenerateExecutable = false,
                OutputAssembly = output,
                WarningLevel = 1
            };
            if (main.GArgs != null) {
                if (CommandLine.Parser.Default.ParseArguments(main.GArgs, options)) {
                    foreach (string refs in options.Refs) {
                        cp.ReferencedAssemblies.Add(refs);
                        Console.WriteLine($"Adding assembly reference {refs}");
                    }
                    if (!options.Optimize) {
                        cp.CompilerOptions = "/optimize";
                    }
                }
            }

            foreach (string file in Directory.GetFiles(_managedFolder)) {
                if (file.EndsWith(".dll", StringComparison.CurrentCulture) && !file.Contains("msc")) {
                    cp.ReferencedAssemblies.Add(file);

                }
            }

            CompilerResults results = provider.CompileAssemblyFromSource(cp, File.ReadAllText(scriptFile));
            foreach (object error in results.Errors) {
                Console.WriteLine(error);
            }
            Console.WriteLine("Compiled script: " + scriptFile + " Sucessfully");
            return cp.OutputAssembly;

        }
    }
}
