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
	class ScriptsCompiler
	{
		private string _pluginsDirectory;
		private string _managedFolder;
		public ScriptsCompiler(string pluginsDirectory, string managedFolder)
		{
			_pluginsDirectory = pluginsDirectory;
			_managedFolder = managedFolder;

			if(!Directory.Exists(_pluginsDirectory))
			{
				Console.WriteLine("Plugins directory: " + _pluginsDirectory + " does not exist!");
				Console.ReadLine();
				return;
			}
		}

        public string CompileScript(string scriptFile)
        {
            CSharpCodeProvider provider = new CSharpCodeProvider();

            string outputPath = "Plugins/" + new FileInfo(scriptFile).Name.Replace(".cs", ".dll");
            CompilerParameters compilerParams = new CompilerParameters
            {
                GenerateExecutable = false,
                OutputAssembly = outputPath,
                WarningLevel = 1,


        };

			foreach (var file in Directory.GetFiles(_managedFolder))
			{
                if (file.EndsWith(".dll") && !file.Contains("msc") && !file.Contains(("System")))
				{
					compilerParams.ReferencedAssemblies.Add(file);
					compilerParams.ReferencedAssemblies.Add("System.Core.dll");

				}
			}

			CompilerResults results = provider.CompileAssemblyFromSource(compilerParams, File.ReadAllText(scriptFile));

			foreach (var error in results.Errors)
			{
				Console.WriteLine(error);
			}

			Console.WriteLine("Compiled script: " + scriptFile);
			return compilerParams.OutputAssembly;
		}
	}
}
