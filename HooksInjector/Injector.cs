using Mono.Cecil;
using Mono.Cecil.Inject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Mono.Cecil.Rocks;

namespace HooksInjector
{
    class Injector
    {
        string _pluginPath;
        AssemblyDefinition _gameAssembly;
        AssemblyDefinition _pluginAssembly;
        public Injector(AssemblyDefinition gameAssembly, AssemblyDefinition pluginAssembly, string pluginPath) {
            this._pluginPath = pluginPath;
            this._gameAssembly = gameAssembly;
            this._pluginAssembly = pluginAssembly;

        }
        public void InjectHook(ScriptsParser.ParsedHook hook, string script) {
            string[] nameSplit = hook.FullName.Split('.');
            string className = hook.FullName.Substring(0, hook.FullName.Substring(0, hook.FullName.Length - 1).LastIndexOf('.'));
            string methodName = nameSplit[nameSplit.Length - 1];

            TypeDefinition methodClassType = _gameAssembly.MainModule.GetType(className);
            if (methodClassType == null) {
                Console.WriteLine("HooksInjector: ERROR: Class " + className + " Was not found in game assembly. Please check the spelling of the class.");
                Console.ReadLine();
                return;
            }

            MethodDefinition method = methodClassType.GetMethod(methodName);

            if (method == null) {
                Console.WriteLine("HooksInjector: ERROR: Method " + methodName + " could not be found in class: " + className + ". Please check the spelling of the method.");
                Console.Read();
                return;

            }
            TypeDefinition classType = null;
            foreach (TypeDefinition type in _pluginAssembly.MainModule.GetTypes()) {
                if (type.Name.Contains(script.Split('.')[0])) {
                    classType = type;
                }

            }

             if (classType == null) {
                Console.WriteLine("HooksInjector: ERROR: No class ending with \"Plugin\" found in " + _pluginPath);
                Console.Read();
                return;
            }

            if (classType.IsNotPublic) {
                classType.IsPublic = true;
            }


            string rawmethodName = hook.FullName.Split('.').Last();
            MethodDefinition hookMethod = classType.GetMethod(methodName);

            if (hookMethod == null) {
                Console.WriteLine("HooksInjector: ERROR: Method " + rawmethodName + " Not found in class " + className);
                Console.ReadLine();
                return;
            }

            try {
                InjectionDefinition injector;
                injector = hook.CanBlock ? method.Parameters.Count > 0 ? new InjectionDefinition(method, hookMethod, InjectFlags.PassInvokingInstance | InjectFlags.PassParametersRef | InjectFlags.ModifyReturn) : new InjectionDefinition(method, hookMethod, InjectFlags.PassInvokingInstance | InjectFlags.ModifyReturn) : method.Parameters.Count > 0 ? new InjectionDefinition(method, hookMethod, InjectFlags.PassInvokingInstance | InjectFlags.PassParametersRef) : new InjectionDefinition(method, hookMethod, InjectFlags.PassInvokingInstance);

                if (hook.HookEnd) {
                    injector.Inject(-1, null, InjectDirection.Before);
                }
                else
                    injector.Inject();
                Console.WriteLine("HooksInjector: Hooked " + rawmethodName + ".");
            }
            catch (Exception e) {
                Console.WriteLine("HooksInjector: ERROR: " + e.ToString() + " Hook definition is probably wrong.");
                Console.ReadLine();
            }


        }
    }
}
