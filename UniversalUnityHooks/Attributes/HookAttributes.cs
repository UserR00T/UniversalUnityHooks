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
			foreach (var hook in attributes[nameof(HookAttributes)])
			{
				hook.TargetData = Cecil.ConvertStringToClassAndMethod(hook.Attribute.ConstructorArguments[0].Value.ToString(), assemblyDefinition);
				if (hook.TargetData == null)
					continue;
				if (Cecil.Inject(hook.TargetData, hook))
					++injectedCorrectly;
			}
			return injectedCorrectly;
		}
	}
}
