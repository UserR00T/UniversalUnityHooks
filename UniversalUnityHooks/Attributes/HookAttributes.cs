using Mono.Cecil;
using System.Collections.Generic;
using System.Linq;

namespace UniversalUnityHooks.Attributes
{
    public sealed class HookAttributes : Attribute
	{
		public HookAttributes(List<AttributeData> attributes, OperationTimer timer) : base(attributes, timer)
        {
        }

		public override AttributesHelper.AddAttributesResponse AddAllFound()
		{
			TempData = AllAttributes.Where(x => x.Attribute.AttributeType.Name == nameof(UniversalUnityHooks.HookAttribute)).ToList();
			if (TempData == null || TempData.Count == 0)
			{
				Program.Chalker.WriteWarning($"No hook attributes were found. Without hook attributes it cannot inject anything. ({Timer.GetElapsedMs}ms)");
				return AttributesHelper.AddAttributesResponse.Info;
			}
			return base.AddAllFound();
		}

		public static int InjectHooks(Dictionary<string, List<AttributeData>> attributes, AssemblyDefinition assemblyDefinition)
		{
			if (!attributes.ContainsKey(nameof(HookAttributes)))
				return 0;
			var injectedCorrectly = 0;
			foreach (var hook in attributes[nameof(HookAttributes)])
			{
				hook.TargetData = Program.Cecil.ConvertStringToClassAndMethod(hook.Attribute.ConstructorArguments[0].Value.ToString(), assemblyDefinition);
				if (hook.TargetData == null)
					continue;
				if (Program.Cecil.Inject(hook.TargetData, hook))
					++injectedCorrectly;
			}
			return injectedCorrectly;
		}
	}
}
