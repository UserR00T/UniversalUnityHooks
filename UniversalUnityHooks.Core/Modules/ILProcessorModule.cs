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
            if (!MethodInfo.IsStatic)
            {
                CliAssert.Fail("The ILProcessorModule method must be static.");
            }
            var parameters = MethodInfo.GetParameters();
            if (parameters.Length != 3)
            {
                CliAssert.Fail("The ILProcessorModule method must exactly have 3 arguments, of types 'ILProcessor', 'ILProcessor', and 'AssemblyDefinition'.");
            }
            var types = new Type[]
            {
                typeof(ILProcessor),
                typeof(MethodDefinition),
                typeof(AssemblyDefinition)
            };
            for (int i = 0; i < types.Length; i++)
            {
                if (parameters[i].ParameterType != types[i])
                {
                    CliAssert.Fail($"Expected parameter {i} to be of type {types[i].FullName}, but was actually {parameters[i].ParameterType.FullName}.");
                }
            }
            MethodInfo.Invoke(null, new object[] { il, targetMethodDefinition, TargetAssembly });
        }
    }
}