using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UniversalUnityHooks.AttributesHelper;
using static UniversalUnityHooks.Util;

namespace UniversalUnityHooks.Attributes
{
	public sealed class AddMethodAttribute : Attribute
	{
		public AddMethodAttribute(List<AttributeData> attributes, OperationTimer timer) : base(attributes, timer) { }
		public override AddAttributesResponse AddAllFound()
		{
			TempData = AllAttributes.Where(x => x.Attribute.AttributeType.Name == nameof(HookAttribute.AddMethodAttribute)).ToList();
			if (TempData == null || TempData.Count == 0)
			{
				ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Warning, $"No add method attributes were found. ({Timer.GetElapsedMs}ms)\r\n");
				return AddAttributesResponse.Info;
			}
			return base.AddAllFound();
		}
		public static int InjectHooks(Dictionary<string, List<AttributeData>> attributes, AssemblyDefinition assemblyDefinition)
		{
			var injectedCorrectly = 0;
			foreach (var hook in attributes[nameof(AddMethodAttribute)])
			{
				// Improve: Add check for multiple args here
				var typeDefinition = Cecil.ConvertStringToClass(hook.Attribute.ConstructorArguments[0].Value.ToString(), assemblyDefinition);
				var methodName = hook.Attribute.ConstructorArguments[1].Value.ToString();
				if (Cecil.MethodExists(typeDefinition, methodName))
				{
					ConsoleHelper.WriteError($"Method \"{methodName}\" Already exists in type {typeDefinition.Name}.");
					continue;
				}
				if (Cecil.InjectNewMethod(typeDefinition, assemblyDefinition, methodName, hook))
					++injectedCorrectly;
			}
			return injectedCorrectly;
		}
	}
}
