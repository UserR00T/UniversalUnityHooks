using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace UniversalUnityHooks
{
    public class AttributesHelper
	{
		public enum AddAttributesResponse
		{
            None,
			Ok,
			Info,
			Error
		}

		public Dictionary<string, List<AttributeData>> InstantiateAndInvoke(List<AttributeData> foundAttributes, OperationTimer timer)
		{
			var returnData = new Dictionary<string, List<AttributeData>>();
			long lastElapsedMs = 0;
			foreach (var type in Util.GetTypesInNamespace(Assembly.GetExecutingAssembly(), "UniversalUnityHooks.Attributes"))
			{
				if (type.BaseType != typeof(Attributes.Attribute))
					continue;
				var instance = (Attributes.Attribute)Activator.CreateInstance(type, new object[] { foundAttributes, timer });
                if (instance.AddAllFound() != AddAttributesResponse.Ok)
                    continue;
                var attributes = instance.Attributes;
                if (attributes == null)
					continue;
				returnData.Add(type.Name, attributes);
				Program.Chalker.WriteMessage($"Attributes found of type {type.Name}: {attributes.Count} ({timer.GetElapsedMs - lastElapsedMs}ms)");
				lastElapsedMs = timer.GetElapsedMs;
			}
			return returnData;
		}

		public List<AttributeData> FindInAssembly(AssemblyDefinition assembly)
		{
			var returnData = new List<AttributeData>();
			foreach (var type in assembly.MainModule.GetTypes())
			{
				foreach (var method in type.Methods)
				{
					var _attributes = method.CustomAttributes;
					if (_attributes == null || _attributes.Count == 0)
						continue;
					for (int i = 0; i < _attributes.Count; i++)
						returnData.Add(new AttributeData(type, method, _attributes[i], assembly));
				}
			}
			return returnData;
		}
	}
}
