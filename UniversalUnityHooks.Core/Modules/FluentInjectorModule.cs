using UniversalUnityHooks.Attributes;
using UniversalUnityHooks.Core.Abstractions;
using UniversalUnityHooks.Core.FluentInjector;

namespace UniversalUnityHooks.Core.Modules
{
    /// <summary>
    /// The module handler for the <see cref="UniversalUnityHooks.Attributes.FluentInjectorAttribute" />
    /// </summary>
    public class FluentInjectorModule : Module<FluentInjectorAttribute>
    {
        /// <inheritdoc/>
        public override void Execute(FluentInjectorAttribute attribute)
        {
            var injector = new Injector(ExecutingAssembly, TargetAssembly);
            if (!MethodInfo.IsStatic)
            {
                CliAssert.Fail("The FluentInjectorModule method must be static.");
            }
            var parameters = MethodInfo.GetParameters();
            if (parameters.Length != 1 || parameters[0].ParameterType != typeof(Injector))
            {
                CliAssert.Fail("The FluentInjectorModule method must exactly have one argument, of type 'Injector'.");
            }
            MethodInfo.Invoke(null, new object[] { injector });
        }
    }
}