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
	public class AddMethodAttribute : IAttribute
	{
		public List<ReturnData<CustomAttribute>> AllAttributes { get; set; }
		public List<ReturnData<CustomAttribute>> Attributes { get; set; }
		public OperationTimer Timer { get; set; }
		public AddMethodAttribute(List<ReturnData<CustomAttribute>> attributes, OperationTimer timer)
		{
			AllAttributes = attributes;
			Timer = timer;
		}
		public AddAttributesResponse AddAllFound()
		{
			var addMethodAttributes = AllAttributes.Where(x => x.Attribute.AttributeType.Name == nameof(HookAttribute.AddMethodAttribute)).ToList();
			if (addMethodAttributes == null || addMethodAttributes.Count == 0)
			{
				ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Warning, $"No add method attributes were found. ({Timer.GetElapsedMs}ms)\r\n");
				return AddAttributesResponse.Info;
			}
			if (addMethodAttributes.Any(x => !x.Type.IsPublic))
			{
				ConsoleHelper.WriteError($"All types (classes that contain the method for example) must be public. ({Timer.GetElapsedMs}ms)\r\n");
				return AddAttributesResponse.Error;
			}
			Attributes = addMethodAttributes;
			return AddAttributesResponse.Ok;
		}
		public int Count => Attributes.Count;
	}
}
