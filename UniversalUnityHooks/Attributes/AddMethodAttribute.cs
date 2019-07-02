using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversalUnityHooks.Attributes
{
    public sealed class AddMethodAttribute : Attribute
	{
		public AddMethodAttribute(List<AttributeData> attributes, OperationTimer timer) : base(attributes, timer) { }

        public override AttributesHelper.AddAttributesResponse AddAllFound()
		{
			TempData = AllAttributes.Where(x => x.Attribute.AttributeType.Name == nameof(UniversalUnityHooks.AddMethodAttribute)).ToList();
			if (TempData == null || TempData.Count == 0)
			{
				Program.Chalker.WriteWarning($"No add method attributes were found. ({Timer.GetElapsedMs}ms)");
				return AttributesHelper.AddAttributesResponse.Info;
			}
			return base.AddAllFound();
		}

		public static int InjectHooks(Dictionary<string, List<AttributeData>> attributes, AssemblyDefinition assemblyDefinition)
		{
			if (!attributes.ContainsKey(nameof(AddMethodAttribute)))
				return 0;
			var injectedCorrectly = 0;
            Console.WriteLine();
			foreach (var hook in attributes[nameof(AddMethodAttribute)])
			{
				// Improve: Add check for multiple args here
				var typeDefinition = Program.Cecil.ConvertStringToClass(hook.Attribute.ConstructorArguments[0].Value.ToString(), assemblyDefinition);
				if (typeDefinition == null)
					continue;
				var methodName = hook.Attribute.ConstructorArguments[1].Value.ToString();
				if (Program.Cecil.MethodExists(typeDefinition, methodName))
				{
					Program.Chalker.WriteError($"Method \"{methodName}\" Already exists in type {typeDefinition.Name}.");
					continue;
				}
				if (Program.Cecil.InjectNewMethod(typeDefinition, assemblyDefinition, methodName, hook))
					++injectedCorrectly;
			}
			return injectedCorrectly;
		}
	}
}
