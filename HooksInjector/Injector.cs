using Mono.Cecil;
using Mono.Cecil.Inject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HooksInjector
{
	class Injector
	{
		private string pluginPath;
		private AssemblyDefinition gameAssembly;
		private AssemblyDefinition pluginAssembly;
		public Injector(AssemblyDefinition _gameAssembly, AssemblyDefinition _pluginAssembly, string _pluginPath)
		{
			pluginPath = _pluginPath;
			gameAssembly = _gameAssembly;
			pluginAssembly = _pluginAssembly;

		}
		public void InjectHook(ScriptsParser.ParsedHook hook)
		{
			var nameSplit = hook.fullName.Split('.');
			var className = nameSplit[0];
			var methodName = nameSplit[1];

			var methodClassType = gameAssembly.MainModule.GetType(className);
			if (methodClassType == null)
			{
				Console.WriteLine("HooksInjector: ERROR: Class " + className + " Was not found in game assembly. Please check the spelling of the class.");
				Console.ReadLine();
				return;
			}

			var method = methodClassType.GetMethod(methodName);

			if (method == null)
			{
				Console.WriteLine("HooksInjector: ERROR: Method " + methodName + " could not be found in class: " + className + ". Please check the spelling of the method.");
				Console.Read();
				return;

			}
			TypeDefinition classType = null;
			foreach (var type in pluginAssembly.MainModule.GetTypes())
			{
				if (type.Name.EndsWith("Plugin", StringComparison.CurrentCulture))
				{
					classType = type;
				}
			}
			if (classType == null)
			{
				Console.WriteLine("HooksInjector: ERROR: No class ending with \"Plugin\" found in " + pluginPath);
				Console.Read();
				return;
			}
			var rawmethodName = hook.fullName.Split('.').Last();
			var hookMethod = classType.GetMethod(methodName);

			if (hookMethod == null)
			{
				Console.WriteLine("HooksInjector: ERROR: Method " + rawmethodName + " Not found in class " + className);
				Console.ReadLine();
				return;
			}
			InjectionDefinition injector;

			try
			{
				if (hook.canBlock)
				{
					if (method.Parameters.Count > 0)
						injector = new InjectionDefinition(method, hookMethod, InjectFlags.PassInvokingInstance | InjectFlags.PassParametersRef | InjectFlags.ModifyReturn);
					else
						injector = new InjectionDefinition(method, hookMethod, InjectFlags.PassInvokingInstance | InjectFlags.ModifyReturn);
				}
				else
				{
					if (method.Parameters.Count > 0)
						injector = new InjectionDefinition(method, hookMethod, InjectFlags.PassInvokingInstance | InjectFlags.PassParametersRef);
					else
						injector = new InjectionDefinition(method, hookMethod, InjectFlags.PassInvokingInstance);
				}

				if (hook.hookEnd)
				{
					injector.Inject(-1, null, InjectDirection.Before);
				}
				else
					injector.Inject();
				Console.WriteLine("HooksInjector: Hooked " + rawmethodName + ".");
			}
			catch (Exception e)
			{
				Console.WriteLine("HooksInjector: ERROR: " + e.ToString() + " Hook definition is probably wrong.");
				Console.ReadLine();
				return;
			}
		}
	}
}
