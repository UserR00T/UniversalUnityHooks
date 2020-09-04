using System;
using System.Reflection;
using Mono.Cecil;
using UniversalUnityHooks.Core.Interfaces;

namespace UniversalUnityHooks.Core.Abstractions
{
    // TODO: figure out how to do inheritdocs
    public abstract class Module<AttributeType> : IModule where AttributeType : Attribute
    {
        public MethodDefinition Method { get; private set; }

        public MethodInfo MethodInfo { get; private set; }

        public TypeDefinition Type { get; private set; }

        public AssemblyDefinition ExecutingAssembly { get; private set; }

        public AssemblyDefinition TargetAssembly { get; private set; }

        public virtual bool IsValidAttribute(CustomAttribute attribute)
        {
            return attribute.AttributeType.FullName == typeof(AttributeType).FullName;
        }

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