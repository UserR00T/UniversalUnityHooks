using Mono.Cecil;
using Mono.Cecil.Inject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UniversalUnityHooks.Util;

namespace UniversalUnityHooks
{
    public static class Cecil
    {
        public static AssemblyDefinition ReadAssembly(string assemblyPath)
        {
            try
            {
                var resolver = new DefaultAssemblyResolver();
                resolver.AddSearchDirectory(Program.managedFolder);
                return AssemblyDefinition.ReadAssembly(assemblyPath, new ReaderParameters { AssemblyResolver = resolver });
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"Cannot load in the assembly {assemblyPath} (Cecil).");
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"Message: {ex.Message}");
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, ex.StackTrace);
                return null;
            }
        }
        public class ReturnData
        {
            public TypeDefinition typeDefinition;
            public MethodDefinition methodDefinition;

            public ReturnData(TypeDefinition typeDefinition, MethodDefinition methodDefinition)
            {
                this.typeDefinition = typeDefinition;
                this.methodDefinition = methodDefinition;
            }
        }
        public static ReturnData ConvertStringToClassAndMethod(string str, AssemblyDefinition targetAssembly)
        {
            var _strSplit = str.Split('.');
            var _className = str.Substring(0, str.Substring(0, str.Length - 1).LastIndexOf('.'));
            var _methodName = _strSplit.Last();
            var typeDefinition = targetAssembly.MainModule.Types.FirstOrDefault(x => x.Name == _className || x.FullName == _className);
            if (typeDefinition == null)
            {
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"Type \"{_className}\" is not found in the target assembly. Please check the spelling of the type and try again.");
                return null;
            }
            var methodDefinition = typeDefinition.GetMethod(_methodName);
            if (methodDefinition == null)
            {
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"Method \"{_methodName}\" not found in class \"{_className}\". inside the target assembly. Please check the spelling of the class and try again.");
                return null;
            }
            return new ReturnData(typeDefinition, methodDefinition);
        }
        internal static bool Inject(ReturnData data, Attributes.ReturnData<CustomAttribute> hook, AssemblyDefinition targertAssembly, AssemblyDefinition currentAssembly)
        {
            try
            {
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Wait, $"Injecting method {data.methodDefinition.Name}..");
                //reflcator
                var injector =
                    hook.method.ReturnType.Name != "Void" 
                    ? data.methodDefinition.Parameters.Count > 0 
                        ? new InjectionDefinition(data.methodDefinition, hook.method, InjectFlags.PassInvokingInstance | InjectFlags.PassParametersRef | InjectFlags.ModifyReturn) 
                        : new InjectionDefinition(data.methodDefinition, hook.method, InjectFlags.PassInvokingInstance | InjectFlags.ModifyReturn) 
                    : data.methodDefinition.Parameters.Count > 0 
                        ? new InjectionDefinition(data.methodDefinition, hook.method, InjectFlags.PassInvokingInstance | InjectFlags.PassParametersRef) 
                        : new InjectionDefinition(data.methodDefinition, hook.method, InjectFlags.PassInvokingInstance);

                if ((bool)hook.attribute.ConstructorArguments[1].Value)
                    injector.Inject(-1, null, InjectDirection.Before);
                else
                    injector.Inject();
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, $"Injected method {data.methodDefinition.Name}.");
                return true;
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"Something went wrong while injecting.");
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"Message: {ex.Message}");
                if (!(ex is InjectionDefinitionException))
                    ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, ex.StackTrace);
            }
            return false;
        }
        internal static bool WriteChanges(AssemblyDefinition targetAssembly)
        {
            try
            {
                targetAssembly.Write(Program.TargetAssembly);
                return true;
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"Something went wrong while writing changes to target assembly.");
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"Message: {ex.Message}");
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, ex.StackTrace);
            }
            return false;
        }
    }
}
