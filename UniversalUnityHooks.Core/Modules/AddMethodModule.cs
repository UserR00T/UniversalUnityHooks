using Mono.Cecil;
using Mono.Cecil.Inject;
using UniversalUnityHooks.Attributes;
using UniversalUnityHooks.Core.Abstractions;

namespace UniversalUnityHooks.Core.Modules
{
    /// <summary>
    /// The module handler for the <see cref="UniversalUnityHooks.Attributes.AddMethodAttribute" />
    /// </summary>
    public class AddMethodModule : Module<AddMethodAttribute>
    {
        /// <inheritdoc/>
        public override void Execute(AddMethodAttribute attribute)
        {
            var targetMethod = attribute?.Method ?? Method.Name;
            var methodDefinition = new MethodDefinition(targetMethod, MethodAttributes.Public, Method.ReturnType)
            {
                IsStatic = Type.IsSealed
            };
            Type.Methods.Add(methodDefinition);
            var flags = GetInjectFlags(methodDefinition, attribute?.Flags);
            var injector = new InjectionDefinition(methodDefinition, Method, attribute?.Flags ?? flags);
            injector.Inject(attribute.StartCode, attribute.Token, attribute.Direction);
        }
    }
}