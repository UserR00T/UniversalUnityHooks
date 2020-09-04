using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Inject;
using UniversalUnityHooks.Attributes;
using UniversalUnityHooks.Core.Abstractions;

namespace UniversalUnityHooks.Core.Modules
{
    /// <summary>
    /// The module handler for the <see cref="UniversalUnityHooks.Attributes.ILProcessorAttribute" />
    /// </summary>
    public class ILProcessorModule : Module<ILProcessorAttribute>
    {
        /// <inheritdoc/>
        public override void Execute(ILProcessorAttribute attribute)
        {
            var targetMethod = attribute?.Method ?? Method.Name;
            var targetMethodDefinition = TargetAssembly.MainModule.GetType(attribute.Type.FullName).GetMethod(targetMethod);
            var il = targetMethodDefinition.Body.GetILProcessor();
            MethodInfo.Invoke(null, new object[] { il, targetMethodDefinition, TargetAssembly });
        }
    }
}