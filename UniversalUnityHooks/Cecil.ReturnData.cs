using Mono.Cecil;

namespace UniversalUnityHooks
{
    public partial class Cecil
    {
        public class ReturnData
        {
            public ReturnData(TypeDefinition typeDefinition, MethodDefinition methodDefinition)
            {
                TypeDefinition = typeDefinition;
                MethodDefinition = methodDefinition;
            }

            public TypeDefinition TypeDefinition { get; }

            public MethodDefinition MethodDefinition { get; }
        }
    }
}
