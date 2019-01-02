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
	public abstract class Attribute
	{
		public virtual AddAttributesResponse AddAllFound()
		{
			if (TempData.Any(x => !x.Type.IsPublic))
			{
				ConsoleHelper.WriteError($"All types (classes that contain the method for example) must be public. ({Timer.GetElapsedMs}ms)\r\n");
				return AddAttributesResponse.Error;
			}
			Attributes = TempData;
			return AddAttributesResponse.Ok;
		}
		internal List<AttributeData> TempData { get; set; } = new List<AttributeData>();
		public List<AttributeData> AllAttributes { get; set; }
		public List<AttributeData> Attributes { get; set; }
		public OperationTimer Timer { get; set; }
		public int Count => Attributes.Count;
		protected Attribute(List<AttributeData> attributes, OperationTimer timer)
		{
			AllAttributes = attributes;
			Timer = timer;
		}
	}
}
