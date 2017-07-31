using Microsoft.CSharp;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HooksInjector
{
	class Program
	{
		/// <summary>
		/// Where script files are located
		/// </summary>
		private const string SCRIPTS_DIRECTORY = "Scripts";
		private const string PLUGINS_DIRECTORY = "Plugins";

		private static string _managedFolder;

		public static void Main(string[] args)
		{
			CreateFiles();

			ScriptsParser parser = new ScriptsParser();
			ScriptsCompiler compiler = new ScriptsCompiler(PLUGINS_DIRECTORY, _managedFolder);

			string gameAssemblyPath = _managedFolder + "\\Assembly-CSharp.dll";

			if (!File.Exists(gameAssemblyPath))
			{
				Console.WriteLine("Can't find: " + gameAssemblyPath);
				Console.ReadLine();
				return;
			}

			if (!File.Exists("Assembly-CSharp.dll"))
			{
				File.Copy(gameAssemblyPath, "Assembly-CSharp.dll");
			}

			string originalAssemblyPath = "Assembly-CSharp.dll";

			AssemblyDefinition gameAssembly = AssemblyDefinition.ReadAssembly(originalAssemblyPath);

			foreach (var scriptFile in Directory.GetFiles(SCRIPTS_DIRECTORY))
			{
				var hooks = parser.GetHooks(scriptFile);
				string pluginFile = compiler.CompileScript(scriptFile);

				if(!File.Exists(pluginFile))
				{
					Console.WriteLine("Plugin: " + pluginFile + " wasn't compiled as promised.");
					Console.ReadLine();
					return;
				}

				Injector injector = new Injector(gameAssembly, AssemblyDefinition.ReadAssembly(pluginFile), pluginFile);
				foreach(var hook in hooks)
				{
					injector.InjectHook(hook);
				}
			}

			gameAssembly.Write(gameAssemblyPath);
			Console.WriteLine("Hooks inserted!");

			foreach (var pluginFile in Directory.GetFiles(PLUGINS_DIRECTORY))
			{
				string pluginDestPath = _managedFolder + "\\" + new FileInfo(pluginFile).Name;
				if(File.Exists(pluginDestPath))
				{
					File.Delete(pluginDestPath);
				}

				File.Copy(pluginFile, pluginDestPath);
			}

			Console.WriteLine("Plugins copied over!");
		}

		/// <summary>
		/// Creates all the necessary directories and files required for HooksInjector to function
		/// </summary>
		private static void CreateFiles()
		{
			if (!Directory.Exists(SCRIPTS_DIRECTORY))
			{
				Directory.CreateDirectory(SCRIPTS_DIRECTORY);
				Console.WriteLine(SCRIPTS_DIRECTORY + " directory created!");
			}

			if (!Directory.Exists(PLUGINS_DIRECTORY))
			{
				Directory.CreateDirectory(PLUGINS_DIRECTORY);
				Console.WriteLine(PLUGINS_DIRECTORY + " directory created!");
			}

			_managedFolder = FindManagedFolder();
		}

		private static string FindManagedFolder()
		{
			foreach (var directory in Directory.EnumerateDirectories(Directory.GetCurrentDirectory()))
			{
				if (directory.EndsWith("_Data"))
				{
					return directory + "\\Managed";
				}
			}

			Console.WriteLine("Managed folder not found!");
			Console.ReadLine();
			return null;
		}
	}
}
