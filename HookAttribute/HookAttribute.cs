using System;

namespace UniversalUnityHooks
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class HookAttribute : Attribute
    {
        private bool AddToEnd { get; }

        private string FullName { get; }

        public HookAttribute(string fullName, bool addToEnd = false)
        {
            FullName = fullName;
            AddToEnd = addToEnd;
        }
    }
}
