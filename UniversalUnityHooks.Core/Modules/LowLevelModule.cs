using UniversalUnityHooks.Attributes;
using UniversalUnityHooks.Core.Abstractions;
using UniversalUnityHooks.Core.LowLevelModule;

namespace UniversalUnityHooks.Core.Modules
{
    /// <summary>
    /// The module handler for the <see cref="UniversalUnityHooks.Attributes.LowLevelModuleAttribute" />
    /// </summary>
    public class LowLevelModule : Module<LowLevelModuleAttribute>
    {
        /// <inheritdoc/>
        public override void Execute(LowLevelModuleAttribute attribute)
        {
            var injector = new Injector(ExecutingAssembly, TargetAssembly);
            if (!MethodInfo.IsStatic)
            {
                CliAssert.Fail("The LowLevelModule method must be static.");
            }
            var parameters = MethodInfo.GetParameters();
            if (parameters.Length != 1 || parameters[0].ParameterType != typeof(Injector))
            {
                CliAssert.Fail("The LowLevelModule method must exactly have one argument, of type 'Injector'.");
            }
            MethodInfo.Invoke(null, new object[] { injector });
        }
    }
}