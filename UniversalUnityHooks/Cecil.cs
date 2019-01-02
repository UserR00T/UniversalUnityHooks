using Mono.Cecil;
using Mono.Cecil.Cil;
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
                ConsoleHelper.WriteError($"Cannot load in the assembly {assemblyPath} (Cecil).");
                ConsoleHelper.WriteError($"Message: {ex.Message}");
                ConsoleHelper.WriteError(ex.StackTrace);
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
		public static bool MethodExists(TypeDefinition type, string methodName) => type.Methods.Any(x => x.Name == methodName || x.FullName == methodName);
		public static bool InjectNewMethod(TypeDefinition type, AssemblyDefinition assembly, string methodName, AttributesHelper.ReturnData<CustomAttribute> hook)
		{
			try
			{
				ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Wait, $"Injecting new method \"{type.Name}.{methodName}\"..");
				var methodDefinition = new MethodDefinition(methodName, MethodAttributes.Public, assembly.MainModule.TypeSystem.Void);
				type.Methods.Add(methodDefinition);
			var il = methodDefinition.Body.GetILProcessor();
			methodDefinition.Body.Instructions.Insert(0, il.Create(OpCodes.Ldnull));
			methodDefinition.Body.Instructions.Insert(1, il.Create(OpCodes.Call, assembly.MainModule.Import(hook.Method)));
			methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, $"Injected new method \"{type.Name}.{methodName}\".");
				return true;
			}
			catch (Exception ex)
			{
				ConsoleHelper.WriteError($"Something went wrong while injecting.");
				ConsoleHelper.WriteError($"Message: {ex.Message}");
				if (!(ex is InjectionDefinitionException))
					ConsoleHelper.WriteError(ex.StackTrace);
				return false;
			}
		}

		public static TypeDefinition ConvertStringToClass(string className, AssemblyDefinition targetAssembly)
		{
			var typeDefinition = targetAssembly.MainModule.Types.FirstOrDefault(x => x.Name == className || x.FullName == className);
			if (typeDefinition == null)
			{
				ConsoleHelper.WriteError($"Type \"{className}\" is not found in the target assembly. Please check the spelling of the type and try again.");
				return null;
			}
			return typeDefinition;
		}
        public static ReturnData ConvertStringToClassAndMethod(string str, AssemblyDefinition targetAssembly)
        {
            var _strSplit = str.Split('.');
            var _className = str.Substring(0, str.Substring(0, str.Length - 1).LastIndexOf('.'));
            var _methodName = _strSplit.Last();
            var typeDefinition = targetAssembly.MainModule.Types.FirstOrDefault(x => x.Name == _className || x.FullName == _className);
            if (typeDefinition == null)
            {
                ConsoleHelper.WriteError($"Type \"{_className}\" is not found in the target assembly. Please check the spelling of the type and try again.");
                return null;
            }
            var methodDefinition = typeDefinition.GetMethod(_methodName);
            if (methodDefinition == null)
            {
                ConsoleHelper.WriteError($"Method \"{_methodName}\" not found in class \"{_className}\". inside the target assembly. Please check the spelling of the class and try again.");
                return null;
            }
            return new ReturnData(typeDefinition, methodDefinition);
        }
        internal static bool Inject(ReturnData data, AttributesHelper.ReturnData<CustomAttribute> hook)
		{
            try
            {
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Wait, $"Injecting method \"{data.typeDefinition.Name}.{data.methodDefinition.Name}\"..");
				//reflcator
				var injector =
                    hook.Method.ReturnType != hook.Assembly.MainModule.TypeSystem.Void
                    ? data.methodDefinition.Parameters.Count > 0 
                        ? new InjectionDefinition(data.methodDefinition, hook.Method, InjectFlags.PassInvokingInstance | InjectFlags.PassParametersRef | InjectFlags.ModifyReturn) 
                        : new InjectionDefinition(data.methodDefinition, hook.Method, InjectFlags.PassInvokingInstance | InjectFlags.ModifyReturn) 
                    : data.methodDefinition.Parameters.Count > 0 
                        ? new InjectionDefinition(data.methodDefinition, hook.Method, InjectFlags.PassInvokingInstance | InjectFlags.PassParametersRef) 
                        : new InjectionDefinition(data.methodDefinition, hook.Method, InjectFlags.PassInvokingInstance);

                if ((bool)hook.Attribute.ConstructorArguments[1].Value)
                    injector.Inject(-1);
                else
                    injector.Inject();
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, $"Injected method \"{data.typeDefinition.Name}.{data.methodDefinition.Name}\".");
                return true;
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Something went wrong while injecting.");
                ConsoleHelper.WriteError($"Message: {ex.Message}");
                if (!(ex is InjectionDefinitionException))
                    ConsoleHelper.WriteError(ex.StackTrace);
            }
            return false;
        }
        internal static bool WriteChanges(AssemblyDefinition targetAssembly)
        {
            try
            {
				targetAssembly.Write(Program.targetAssembly);
                return true;
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"Something went wrong while writing changes to target assembly.");
                ConsoleHelper.WriteError($"Message: {ex.Message}");
                ConsoleHelper.WriteError(ex.StackTrace);
            }
            return false;
        }
    }
}
