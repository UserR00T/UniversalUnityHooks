using System.Collections.Generic;
using System.Linq;

namespace UniversalUnityHooks.Attributes
{
    public abstract class Attribute
	{
		public virtual AttributesHelper.AddAttributesResponse AddAllFound()
		{
			if (TempData.Any(x => !x.Type.IsPublic))
			{
				Program.Chalker.WriteError($"All types (classes that contain the method for example) must be public. ({Timer.GetElapsedMs}ms)\r\n");
				return AttributesHelper.AddAttributesResponse.Error;
			}
			Attributes = TempData;
			return AttributesHelper.AddAttributesResponse.Ok;
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
