using System;
using Mono.Cecil.Inject;

namespace UniversalUnityHooks.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class AddMethodAttribute : Attribute
    {
        // TODO: docs
        public Type Type { get; }

        public string Method { get; }

        public InjectFlags Flags { get; set; }

        public int StartCode { get; set; }

        public object Token { get; set; }

        public InjectDirection Direction { get; set; } = InjectDirection.Before;

        public AddMethodAttribute(Type type)
        {
            Type = type;
        }

        public AddMethodAttribute(Type type, string method) : this(type)
        {
            Method = method;
        }

        public AddMethodAttribute(Type type, string method, InjectFlags flags) : this(type, method)
        {
            Flags = flags;
        }
    }
}
