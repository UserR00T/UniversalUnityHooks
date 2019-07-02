using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Inject;
using System;
using System.Linq;

namespace UniversalUnityHooks
{
    public partial class Cecil
    {
		public bool MethodExists(TypeDefinition type, string methodName) => type.Methods.Any(x => x.Name == methodName || x.FullName == methodName);

        public bool InjectNewMethod(TypeDefinition type, AssemblyDefinition assembly, string methodName, AttributeData hook)
		{
			try
			{
				Program.Chalker.WriteWait($"Injecting new method \"{type.Name}.{methodName}\"..");
				var methodDefinition = new MethodDefinition(methodName, MethodAttributes.Public, assembly.MainModule.TypeSystem.Void)
				{
					IsStatic = type.IsSealed
				};
				type.Methods.Add(methodDefinition);
				var il = methodDefinition.Body.GetILProcessor();
				if (!type.IsSealed)
					methodDefinition.Body.Instructions.Add(il.Create(OpCodes.Ldarg_0));
				methodDefinition.Body.Instructions.Add(il.Create(OpCodes.Call, assembly.MainModule.Import(hook.Method)));
				methodDefinition.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
				Program.Chalker.WriteSuccess($"Injected new method \"{type.Name}.{methodName}\".");
				return true;
			}
			catch (Exception ex)
			{
				Program.Chalker.WriteError($"Something went wrong while injecting.");
				Program.Chalker.WriteError($"Message: {ex.Message}");
				if (!(ex is InjectionDefinitionException))
					Program.Chalker.WriteError(ex.StackTrace);
				return false;
			}
		}

		public TypeDefinition ConvertStringToClass(string className, AssemblyDefinition targetAssembly)
		{
			var typeDefinition = targetAssembly.MainModule.Types.FirstOrDefault(x => x.Name == className || x.FullName == className);
			if (typeDefinition == null)
			{
				Program.Chalker.WriteError($"Type \"{className}\" is not found in the target assembly. Please check the spelling of the type and try again.");
				return null;
			}
			return typeDefinition;
		}

        public ReturnData ConvertStringToClassAndMethod(string str, AssemblyDefinition targetAssembly)
        {
            var _strSplit = str.Split('.');
            var _className = str.Substring(0, str.Substring(0, str.Length - 1).LastIndexOf('.'));
            var _methodName = _strSplit.Last();
            var typeDefinition = targetAssembly.MainModule.Types.FirstOrDefault(x => x.Name == _className || x.FullName == _className);
            if (typeDefinition == null)
            {
                Program.Chalker.WriteError($"Type \"{_className}\" is not found in the target assembly. Please check the spelling of the type and try again.");
                return null;
            }
            var methodDefinition = typeDefinition.GetMethod(_methodName);
            if (methodDefinition == null)
            {
                Program.Chalker.WriteError($"Method \"{_methodName}\" not found in class \"{_className}\". inside the target assembly. Please check the spelling of the class and try again.");
                return null;
            }
            return new ReturnData(typeDefinition, methodDefinition);
        }

        internal bool Inject(ReturnData data, AttributeData hook)
		{
            try
            {
                Program.Chalker.WriteWait($"Injecting method \"{data.TypeDefinition.Name}.{data.MethodDefinition.Name}\"..");
                var flags = InjectFlags.PassInvokingInstance;
                if (data.MethodDefinition.Parameters.Count > 0)
                    flags |= InjectFlags.PassParametersRef;
                if (hook.Method.ReturnType != hook.Assembly.MainModule.TypeSystem.Void)
                    flags |= InjectFlags.ModifyReturn;

                var injector = new InjectionDefinition(data.MethodDefinition, hook.Method, flags);
                if ((bool)hook.Attribute.ConstructorArguments[1].Value)
                    injector.Inject(-1);
                else
                    injector.Inject();
                Program.Chalker.WriteSuccess($"Injected method \"{data.TypeDefinition.Name}.{data.MethodDefinition.Name}\".");
                return true;
            }
            catch (Exception ex)
            {
                Program.Chalker.WriteError("Something went wrong while injecting.");
                Program.Chalker.WriteError($"Message: {ex.Message}");
                if (!(ex is InjectionDefinitionException))
                    Program.Chalker.WriteError(ex.StackTrace);
            }
            return false;
        }

        internal bool WriteChanges(AssemblyDefinition targetAssembly, string targetName)
        {
            try
            {
				targetAssembly.Write(targetName);
                return true;
            }
            catch (Exception ex)
            {
                Program.Chalker.WriteError("Something went wrong while writing changes to target assembly.");
                Program.Chalker.WriteError($"Message: {ex.Message}");
                Program.Chalker.WriteError(ex.StackTrace);
            }
            return false;
        }
    }
}
