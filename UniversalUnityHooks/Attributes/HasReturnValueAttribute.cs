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
	public sealed class HasReturnValueAttribute : Attribute
	{
		public HasReturnValueAttribute(List<AttributeData> attributes, OperationTimer timer) : base(attributes, timer) { }
		public override AddAttributesResponse AddAllFound()
		{
			TempData = AllAttributes.Where(x => x.Attribute.AttributeType.Name == nameof(HookAttribute.HasReturnValueAttribute)).ToList();
			if (TempData == null || TempData.Count == 0)
			{
				ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Warning, $"No has return value attributes were found. ({Timer.GetElapsedMs}ms)");
				return AddAttributesResponse.Info;
			}
			if (TempData.Any(x=>x.Method.ReturnType == x.Assembly.MainModule.TypeSystem.Void))
			{
				ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"You cannot use void as return value for this attribute. ({Timer.GetElapsedMs}ms)");
				return AddAttributesResponse.Info;
			}
			return base.AddAllFound();
		}
		public static int InjectHooks(Dictionary<string, List<AttributeData>> attributes, AssemblyDefinition assemblyDefinition)
		{
			if (!attributes.ContainsKey(nameof(HookAttributes)) || !attributes.ContainsKey(nameof(HasReturnValueAttribute)))
				return 0;
			var injectedCorrectly = 0;
			var hookAttributes = new Dictionary<MethodDefinition, AttributeData>();
			hookAttributes = attributes[nameof(HookAttributes)].ToDictionary(x => x.Method, y => y);
			ConsoleHelper.WriteNewline();
			foreach (var hook in attributes[nameof(HasReturnValueAttribute)])
			{
				if (!hookAttributes.TryGetValue(hook.Method, out var hookAttribute))
				{
					ConsoleHelper.WriteError($"Cannot get hook attribute for method {hook.Method.Name}. Did you decorate that method with the [HookAttribute] method?");
					continue;
				}
				if (hook.Method.ReturnType != hookAttribute.TargetData.methodDefinition.ReturnType)
				{
					ConsoleHelper.WriteError($"Return types of {hook.Method.Name} and {hookAttribute.TargetData.methodDefinition.Name} do not match. They need to match to add the [HasReturnValue] attribute.");
					continue;
				}
				var il = hookAttribute.TargetData.methodDefinition.Body.GetILProcessor();
				var firstInstruction = il.Body.Instructions.First();

				ConsoleHelper.WriteMessage($"Mdef: {hookAttribute.TargetData.methodDefinition} | Tdef: {hookAttribute.TargetData.typeDefinition}");
			}
			return injectedCorrectly;
		}
	}
}
