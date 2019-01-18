using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UniversalUnityHooks.AttributesHelper;
using static UniversalUnityHooks.Util;
namespace UniversalUnityHooks.Attributes
{
	public sealed class HookAttributes : Attribute
	{
		public HookAttributes(List<AttributeData> attributes, OperationTimer timer) : base(attributes, timer) { }
		public override AddAttributesResponse AddAllFound()
		{
			TempData = AllAttributes.Where(x => x.Attribute.AttributeType.Name == nameof(HookAttribute.HookAttribute)).ToList();
			if (TempData == null || TempData.Count == 0)
			{
				ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Warning, $"No hook attributes were found. Without hook attributes it cannot inject anything. ({Timer.GetElapsedMs}ms)");
				return AddAttributesResponse.Info;
			}
			return base.AddAllFound();
		}
		public static int InjectHooks(Dictionary<string, List<AttributeData>> attributes, AssemblyDefinition assemblyDefinition)
		{
			if (!attributes.ContainsKey(nameof(HookAttributes)))
				return 0;
			var injectedCorrectly = 0;
			var returnValueAttributes = new Dictionary<MethodDefinition, AttributeData>();
			returnValueAttributes = attributes[nameof(HasReturnValueAttribute)].ToDictionary(x => x.Method, y => y);
			foreach (var hook in attributes[nameof(HookAttributes)])
			{
				hook.TargetData = Cecil.ConvertStringToClassAndMethod(hook.Attribute.ConstructorArguments[0].Value.ToString(), assemblyDefinition);
				if (hook.TargetData == null)
					continue;
				if (!returnValueAttributes.ContainsKey(hook.Method))
				{
					if (Cecil.Inject(hook.TargetData, hook))
						++injectedCorrectly;
					continue;
				}
				ConsoleHelper.WriteMessage($"F1: {hook.Method.FullName}: {hook.Method.ReturnType.Name} | F2: {hook.TargetData.methodDefinition.FullName}: {hook.TargetData.methodDefinition.ReturnType.Name}");
				if (hook.Method.ReturnType.Name != hook.TargetData.methodDefinition.ReturnType.Name)
				{
					ConsoleHelper.WriteError($"Return types of {hook.Method.Name} and {hook.TargetData.methodDefinition.Name} do not match. They need to match to add the [HasReturnValue] attribute.");
					continue;
				}
				ConsoleHelper.WriteMessage($"Mdef: {hook.TargetData.methodDefinition} | Tdef: {hook.TargetData.typeDefinition}");
				var il = hook.TargetData.methodDefinition.Body.GetILProcessor();
				var firstInstruction = il.Body.Instructions.First();
				var method = hook.TargetData.methodDefinition;
				Instruction lastInstruction = null;
				Instruction currInstruction = null;
				if (!method.IsStatic && !method.DeclaringType.IsSealed)
				{
					currInstruction = il.Create(OpCodes.Ldarg_0);
					if (firstInstruction != null)
						il.InsertBefore(firstInstruction, currInstruction);
					else
						il.Append(currInstruction);
				}
				lastInstruction = currInstruction;
				currInstruction = il.Create(OpCodes.Call, assemblyDefinition.MainModule.Import(hook.Method));
				il.InsertAfter(lastInstruction ?? firstInstruction, currInstruction);
				lastInstruction = currInstruction;
				il.InsertAfter(lastInstruction, Instruction.Create(OpCodes.Ret));
				injectedCorrectly += 2; // HookAttribute and HasReturnValue both succeeded, add two
			}
			return injectedCorrectly;
		}
	}
}
