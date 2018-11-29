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
    class Attributes
    {
        public class ReturnData<AttributeType>
        {
            public ReturnData(TypeDefinition type, MethodDefinition method, AttributeType attribute, AssemblyDefinition assembly)
            {
                this.type = type;
                this.method = method;
                this.attribute = attribute;
                this.assembly = assembly;
            }
            public AssemblyDefinition assembly;
            public TypeDefinition type;
            public MethodDefinition method;
            public AttributeType attribute;
        }
        public static List<ReturnData<CustomAttribute>> GetAllAttributes(AssemblyDefinition assembly)
        {
            var returnData = new List<ReturnData<CustomAttribute>>();
            var types = assembly.MainModule.GetTypes();
            foreach (var type in types)
            {
                foreach (var method in type.Methods)
                {
                    var _attributes = method.CustomAttributes;
                    if (_attributes == null || _attributes.Count == 0)
                        continue;
                    for (int i = 0; i < _attributes.Count; i++)
                        returnData.Add(new ReturnData<CustomAttribute>(type, method, _attributes[i], assembly));
                }
            }
            return returnData;
        }
    }
}
