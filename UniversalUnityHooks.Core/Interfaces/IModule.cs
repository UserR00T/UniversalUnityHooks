using System.Reflection;
using Mono.Cecil;

namespace UniversalUnityHooks.Core.Interfaces
{
    /// <summary>
    /// A non generic interface for the <see cref="UniversalUnityHooks.Core.Abstractions.Module{AttributeType}" /> type. This is used to create a list of modules and enumerate over them.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// The method definition where the attribute was found upon. This is the Mono.Cecil type.
        /// </summary>
        /// <value></value>
        MethodDefinition Method { get; }

        /// <summary>
        /// The method definition where the attribute was found upon. This is the native .NET type. Used to invoke the method if needed.
        /// </summary>
        /// <value></value>
        MethodInfo MethodInfo { get; }

        /// <summary>
        /// The type associated to the method definition. This the Mono.Cecil type.
        /// </summary>
        /// <value></value>
        TypeDefinition Type { get; }

        /// <summary>
        /// The assembly the method definition is associated to.
        /// </summary>
        /// <value></value>
        AssemblyDefinition ExecutingAssembly { get; }

        /// <summary>
        /// The target assembly that the module will act upon.
        /// </summary>
        /// <value></value>
        AssemblyDefinition TargetAssembly { get; }

        /// <summary>
        /// Checks if the provided attribute is a valid input for this module.
        /// </summary>
        /// <param name="attribute">The provided attribute.</param>
        /// <returns>Returns <c>true</c> if the attribute will be handled by this module, <c>false</c>, if not.</returns>
        bool IsValidAttribute(CustomAttribute attribute);

        /// <summary>
        /// Execute the module with the provided input data.
        /// </summary>
        /// <param name="method">The method definition where the attribute was found upon. This is the Mono.Cecil type.</param>
        /// <param name="methodInfo">The method definition where the attribute was found upon. This is the native .NET type. Used to invoke the method if needed.</param>
        /// <param name="type">The type associated to the method definition. This is the Mono.Cecil type.</param>
        /// <param name="executingAssembly">The assembly the method definition is associated to.</param>
        /// <param name="targetAssembly">The target assembly that the module will act upon.</param>
        void Execute(MethodDefinition method, MethodInfo methodInfo, TypeDefinition type, AssemblyDefinition executingAssembly, AssemblyDefinition targetAssembly);
    }
}