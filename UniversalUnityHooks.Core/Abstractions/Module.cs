using System;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Inject;
using UniversalUnityHooks.Core.Interfaces;

namespace UniversalUnityHooks.Core.Abstractions
{
    /// <summary>
    /// A abstract type for the <see cref="UniversalUnityHooks.Core.Interfaces.IModule"/> type.
    /// </summary>
    /// <typeparam name="AttributeType">The attribute associated to the module. This is used to get a statically typed variable to the overridden method.</typeparam>
    public abstract class Module<AttributeType> : IModule where AttributeType : Attribute
    {
        /// <inheritdoc/>
        public MethodDefinition Method { get; private set; }

        /// <inheritdoc/>
        public MethodInfo MethodInfo { get; private set; }

        /// <inheritdoc/>
        public TypeDefinition Type { get; private set; }

        /// <inheritdoc/>
        public AssemblyDefinition ExecutingAssembly { get; private set; }

        /// <inheritdoc/>
        public AssemblyDefinition TargetAssembly { get; private set; }

        /// <inheritdoc/>
        public virtual bool IsValidAttribute(CustomAttribute attribute)
        {
            return attribute.AttributeType.FullName == typeof(AttributeType).FullName;
        }

        /// <summary>
        /// Gets the inject flags for this specified scenario. This will be based on the supplied target method and other variables.
        /// </summary>
        /// <param name="targetMethodDefinition">The target type to inspect and to determine what injectflags are required.</param>
        /// <param name="overrideFlags">If this value is specified, use this instead of a generated InjectFlags list.</param>
        /// <returns><see cref="Mono.Cecil.Inject.InjectFlags"/></returns>
        public InjectFlags GetInjectFlags(MethodDefinition targetMethodDefinition, InjectFlags? overrideFlags)
        {
            InjectFlags flags = default;
            // If target method and target type is not static, append PassInvokingInstance to the flags.
            if (!targetMethodDefinition.IsStatic && !Type.IsSealed)
            {
                flags = InjectFlags.PassInvokingInstance;
            }
            // If the target method has parameters, append PassParametersRef to the flags.
            if (targetMethodDefinition.Parameters.Count > 0)
            {
                flags |= InjectFlags.PassParametersRef;
            }
            // If the target method return type is *not* void, append ModifyReturn to the flags.
            if (Method.ReturnType.FullName != TargetAssembly.MainModule.TypeSystem.Void.FullName)
            {
                flags |= InjectFlags.ModifyReturn;
            }

            // Override the flags if 'overrideFlags' was provided.
            if (overrideFlags != InjectFlags.None)
            {
                flags = (InjectFlags)overrideFlags;
            }
            return flags;
        }

        /// <inheritdoc/>
        public void Execute(MethodDefinition method, MethodInfo methodInfo, TypeDefinition type, AssemblyDefinition executingAssembly, AssemblyDefinition targetAssembly)
        {
            Method = method;
            MethodInfo = methodInfo;
            Type = type;
            ExecutingAssembly = executingAssembly;
            TargetAssembly = targetAssembly;
            Execute(methodInfo.GetCustomAttribute<AttributeType>());
        }

        /// <summary>
        /// Execute the module with the strongly typed <typeparam name="AttributeType"/>. This method should be executed from inside <see cref="UniversalUnityHooks.Core.Abstractions.Module{AttributeType}.Execute(MethodDefinition, MethodInfo, TypeDefinition, AssemblyDefinition, AssemblyDefinition)" />
        /// </summary>
        /// <param name="attribute"></param>
        public abstract void Execute(AttributeType attribute);
    }
}