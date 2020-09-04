using System;
using Mono.Cecil.Inject;

namespace UniversalUnityHooks.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class HookAttribute : Attribute
    {
        // TODO: docs
        public string FullPath { get; }

        public Type Type { get; }

        public string Method { get; }

        public InjectFlags Flags { get; set; }

        public int StartCode { get; set; }

        public object Token { get; set; }

        public InjectDirection Direction { get; set; } = InjectDirection.Before;

        public HookAttribute(string path)
        {
            FullPath = path;
        }

        public HookAttribute(Type type)
        {
            Type = type;
        }

        public HookAttribute(Type type, string method) : this(type)
        {
            Method = method;
        }

        public HookAttribute(Type type, string method, InjectFlags flags) : this(type, method)
        {
            Flags = flags;
        }
    }
}
