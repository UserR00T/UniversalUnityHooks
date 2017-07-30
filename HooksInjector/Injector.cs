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
		private string _pluginPath;
		private AssemblyDefinition _gameAssembly;
		private AssemblyDefinition _pluginAssembly;
		public Injector(AssemblyDefinition gameAssembly, AssemblyDefinition pluginAssembly, string pluginPath)
		{
			_pluginPath = pluginPath;
			_gameAssembly = gameAssembly;
			_pluginAssembly = pluginAssembly;
		}

		public void InjectHook(ScriptsParser.ParsedHook hook)
		{
			var nameSplit = hook.fullName.Split('.');
			var className = nameSplit[0];
			var methodName = nameSplit[1];

			var classType = _gameAssembly.MainModule.GetType(className);
			if(classType == null)
			{
				Console.WriteLine(className + " class not found in game assembly!");
				Console.ReadLine();
				return;
			}

			var method = classType.GetMethod(methodName);

			if (method == null)
			{
				Console.WriteLine(methodName + " method not found in " + className + "!");
				Console.ReadLine();
				return;
			}

			TypeDefinition pluginClassType = null;
			foreach(var type in _pluginAssembly.MainModule.GetTypes())
			{
				if(type.Name.EndsWith("Plugin"))
				{
					pluginClassType = type;
				}
			}

			if(pluginClassType == null)
			{
				Console.WriteLine("No plugin class ending with \"Plugin\" found in " + _pluginPath + "!");
				Console.ReadLine();
				return;
			}

			var rawMethodName = hook.fullName.Split('.').Last();
			var hookMethod = pluginClassType.GetMethod(rawMethodName);

			if (hookMethod == null)
			{
				Console.WriteLine(pluginClassType.Name + " doesn't contain method: " + rawMethodName);
				Console.ReadLine();
				return;
			}

			InjectionDefinition injector;

			try
			{
				if (hook.canBlock)
				{
					if(method.Parameters.Count > 0)
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

				Console.WriteLine(rawMethodName + " hooked!");
			}
			catch(Exception e)
			{
				Console.WriteLine("Hook definition is wrong!");
				Console.WriteLine(e.ToString());
				Console.ReadLine();
				return;
			}
		}
	}
}
