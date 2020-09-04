using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Inject;
using UniversalUnityHooks.Attributes;
using UniversalUnityHooks.Core.Abstractions;

namespace UniversalUnityHooks.Core.Modules
{
    /// <summary>
    /// The module handler for the <see cref="UniversalUnityHooks.Attributes.HookAttribute" />
    /// </summary>
    public class HookModule : Module<HookAttribute>
    {
        /// <inheritdoc/>
        public override void Execute(HookAttribute attribute)
        {
            var targetMethod = attribute?.Method ?? Method.Name;
            MethodDefinition targetMethodDefinition = null;
            if (!string.IsNullOrWhiteSpace(attribute.FullPath))
            {
                var type = TargetAssembly.MainModule.Types.FirstOrDefault(x => attribute.FullPath.StartsWith(x.FullName) || attribute.FullPath.StartsWith(x.Name));
                targetMethodDefinition = type.Methods.FirstOrDefault(x => attribute.FullPath.EndsWith(x.FullName) || attribute.FullPath.EndsWith(x.Name));
            }
            else
            {
                targetMethodDefinition = TargetAssembly.MainModule.GetType(attribute.Type.FullName).GetMethod(targetMethod);
            }

            InjectFlags flags = default;
            // If target method and target type is not static, append PassInvokingInstance to flags.
            if (!targetMethodDefinition.IsStatic && !Type.IsSealed)
            {
                flags = InjectFlags.PassInvokingInstance;
            }
            if (targetMethodDefinition.Parameters.Count > 0)
            {
                flags |= InjectFlags.PassParametersRef;
            }
            if (Method.ReturnType.FullName != TargetAssembly.MainModule.TypeSystem.Void.FullName)
            {
                flags |= InjectFlags.ModifyReturn;
            }

            if (attribute?.Flags != InjectFlags.None)
            {
                flags = attribute.Flags;
            }

            var injector = new InjectionDefinition(targetMethodDefinition, Method, flags);
            injector.Inject(attribute.StartCode, attribute.Token, attribute.Direction);
        }
    }
}