using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static UniversalUnityHooks.Util;

namespace UniversalUnityHooks
{
	public static class AttributesHelper
	{
		public enum AddAttributesResponse
		{
			Ok,
			Info,
			Error
		}
		public static Dictionary<string, List<AttributeData>> FindAndInvokeAllAttributes(List<AttributeData> allAttributes, OperationTimer timer)
		{
			var typelist = GetTypesInNamespace(Assembly.GetExecutingAssembly(), "UniversalUnityHooks.Attributes");
			var returnData = new Dictionary<string, List<AttributeData>>();
			foreach (var type in typelist)
			{
				if (type.BaseType != typeof(Attributes.Attribute))
					continue;
				var instance = Activator.CreateInstance(type, new object[] { allAttributes, timer });
				if (GetAndInvokeAddAllFoundMethod(type, instance))
					continue;
				var attributes = GetAttributeProperty(type, instance);
				if (attributes == null)
					continue;
				returnData.Add(type.Name, attributes);
				ConsoleHelper.WriteMessage($"Attributes found of type {type.Name}: {attributes.Count}");
			}
			return returnData;
		}
		static bool GetAndInvokeAddAllFoundMethod(Type type, object instance)
		{
			var method = type.GetMethod("AddAllFound");
			if (method == null)
				return true;
			if (method.ReturnType != typeof(AddAttributesResponse))
				return true;
			var result = (AddAttributesResponse)method.Invoke(instance, null);
			if (result != AddAttributesResponse.Ok)
				return true;
			return false;
		}
		static List<AttributeData> GetAttributeProperty(Type type, object instance)
		{
			var _attributes = type.GetProperty(nameof(Attributes)).GetValue(instance);
			if (_attributes == null)
				return null;
			var attributes = (List<AttributeData>)_attributes;
			if (attributes.Count == 0)
				return null;
			return attributes;
		}
		public static List<AttributeData> GetAllAttributesInAssembly(AssemblyDefinition assembly)
		{
			var returnData = new List<AttributeData>();
			var types = assembly.MainModule.GetTypes();
			foreach (var type in types)
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
		public class AttributeData
		{
			public AttributeData(TypeDefinition type, MethodDefinition method, CustomAttribute attribute, AssemblyDefinition assembly)
			{
				Type = type;
				Method = method;
				Attribute = attribute;
				Assembly = assembly;
			}
			public AssemblyDefinition Assembly { get; set; }
			public TypeDefinition Type { get; set; }
			public MethodDefinition Method { get; set; }
			public CustomAttribute Attribute { get; set; }
		}
	}
}
