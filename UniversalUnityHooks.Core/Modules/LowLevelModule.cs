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
            MethodInfo.Invoke(null, new object[] { injector });
        }
    }
}