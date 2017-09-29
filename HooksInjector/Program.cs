using System;
using System.IO;
using Mono.Cecil;
using CommandLine;
namespace HooksInjector
{
	class Options
	{
		[Option('r', "ref", Required = false, HelpText = "Adds external assembly references, from the Managed folder, GAC, or specified path.")]
		public string[] refs { get; set; }

		[Option('o', "nooptimize", HelpText = "Turns off Compiler Optimization, this can be useful for debugging.")]
		public bool optimize { get; set; }

		[ParserState]
		public IParserState LastParserState { get; set; }
	}

	public class Program
	{
		public const string scriptsDir = "Scripts";
		public const string pluginsDir = "Plugins";
		public static string managedFolder;
		public static string[] refAssemblys;
		public string[] gArgs;

		public static void Main(string[] args)
		{
			CreateFiles();
			managedFolder = GetManaged();
			var program = new Program()
			{
				gArgs = args
			};
			var parser = new ScriptsParser();
			var compiler = new ScriptsCompiler(pluginsDir, managedFolder);
			string assemblyPath = managedFolder + "/Assembly-CSharp.dll";
			if (!File.Exists(assemblyPath))
			{
				Console.WriteLine("HooksInjector: ERROR: Could not find game assembly. Make sure you're running from the game's root dir.");
				Console.Read();
				return;
			}
			if (!File.Exists("Assembly-CSharp.dll"))
			{
				File.Copy(assemblyPath, "Assembly-CSharp.dll");

			}
			string origAssembly = "Assembly-CSharp.dll";
			var gameAssembly = AssemblyDefinition.ReadAssembly(origAssembly);

			foreach (var scriptfile in Directory.GetFiles(scriptsDir))
			{
				var hooks = parser.GetHooks(scriptfile);
				string pluginFile = compiler.CompileScript(scriptfile);

				if (!File.Exists(pluginFile))
				{
					Console.WriteLine("HooksInjector: ERROR: " + pluginFile + "Was not compiled sucessfully.");
					Console.ReadLine();
					return;

				}

				var injector = new Injector(gameAssembly, AssemblyDefinition.ReadAssembly(pluginFile), pluginFile);
				foreach (var hook in hooks)
				{
					injector.InjectHook(hook);
				}
			}
			gameAssembly.Write(assemblyPath);
			Console.WriteLine("HooksInjector: Hooks inserted sucessfully!");

			foreach (var plugin in Directory.GetFiles(pluginsDir))
			{
				string pluginDest = managedFolder + "/" + new FileInfo(plugin).Name;
				if (File.Exists(pluginDest))
				{
					File.Delete(pluginDest);
				}
				File.Copy(plugin, pluginDest);
			}
			Console.WriteLine("HooksInjector: Plugins copied sucessfully!");
		}

		private static void CreateFiles()
		{
			if (!Directory.Exists(scriptsDir))
			{
				Directory.CreateDirectory(scriptsDir);
				Console.WriteLine("Scripts Directory created");
			}
			if (!Directory.Exists(pluginsDir))
			{
				Directory.CreateDirectory(pluginsDir);
				Console.WriteLine("Plugins Directory created");
			}
		}
		private static string GetManaged()
		{
			foreach (var directory in Directory.EnumerateDirectories((Directory.GetCurrentDirectory())))
			{
				if (directory.EndsWith("_Data", StringComparison.CurrentCulture))
				{
					return directory + "/Managed";
				}
			}
			Console.WriteLine("HooksInjector: ERROR: Managed folder not found. Place HooksInjector in $GameDir");
			return null;
		}

	}
}
