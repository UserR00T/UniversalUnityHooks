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
	public class HookAttributes : IAttribute
	{
		public List<ReturnData<CustomAttribute>> AllAttributes { get; set; }
		public List<ReturnData<CustomAttribute>> Attributes { get; set; }
		public OperationTimer Timer { get; set; }
		public HookAttributes(List<ReturnData<CustomAttribute>> attributes, OperationTimer timer)
		{
			AllAttributes = attributes;
			Timer = timer;
		}
		public AddAttributesResponse AddAllFound()
		{
			var hookAttributes = AllAttributes.Where(x => x.Attribute.AttributeType.Name == nameof(HookAttribute.HookAttribute)).ToList();
			if (hookAttributes == null || hookAttributes.Count == 0)
			{
				ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Warning, $"No hook attributes were found. Without hook attributes it cannot inject anything. ({Timer.GetElapsedMs}ms)\r\n");
				return AddAttributesResponse.Info;
			}
			if (hookAttributes.Any(x => !x.Type.IsPublic))
			{
				ConsoleHelper.WriteError($"All types (classes that contain the method for example) must be public. ({Timer.GetElapsedMs}ms)\r\n");
				return AddAttributesResponse.Error;
			}
			Attributes = hookAttributes;
			return AddAttributesResponse.Ok;
		}
		public int Count => Attributes.Count;
	}
}
