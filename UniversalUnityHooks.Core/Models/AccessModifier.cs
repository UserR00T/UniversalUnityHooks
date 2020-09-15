using System;

namespace UniversalUnityHooks.Core.Models
{
    [Flags]
    public enum AccessModifier
    {
        Unknown,

        Private,

        Public,

        Virtual,

        Assignable,
    }
}