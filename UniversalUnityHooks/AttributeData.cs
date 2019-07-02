using Mono.Cecil;

namespace UniversalUnityHooks
{
    public class AttributeData
    {
        public AttributeData(TypeDefinition type, MethodDefinition method, CustomAttribute attribute, AssemblyDefinition assembly)
        {
            Type = type;
            Method = method;
            Attribute = attribute;
            Assembly = assembly;
        }

        public Cecil.ReturnData TargetData { get; set; }

        public AssemblyDefinition Assembly { get; set; }

        public TypeDefinition Type { get; set; }

        public MethodDefinition Method { get; set; }

        public CustomAttribute Attribute { get; set; }
    }
}
