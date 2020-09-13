using System;
using System.Reflection;
using Mono.Cecil;
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