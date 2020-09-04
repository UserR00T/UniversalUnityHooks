using Mono.Cecil.Inject;

namespace UniversalUnityHooks.Core.Models
{
    /// <summary>
    /// Data to be used when hooking into a target method.
    /// </summary>
    public class HookData
    {
        /// <summary>
        /// The flags to provide towards the injector. By leaving this value <c>default</c> UUH will try its best to select the default flag values by inspecting the target method and hook method.
        /// </summary>
        /// <value></value>
        public InjectFlags Flags { get; set; }

// TODO: better docs
        /// <summary>
        /// The start code for the hook.
        /// </summary>
        /// <value></value>
        public int StartCode { get; set; }

        /// <summary>
        /// The token for the hook.
        /// </summary>
        /// <value></value>
        public object Token { get; set; }

        /// <summary>
        /// The direction to inject the hook into. by default the hook shall be injected in the front of the target method.
        /// </summary>
        /// <value></value>
        public InjectDirection Direction { get; set; } = InjectDirection.Before;
    }
}