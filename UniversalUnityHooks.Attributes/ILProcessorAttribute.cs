using System;

namespace UniversalUnityHooks.Attributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class ILProcessorAttribute : Attribute
    {
        // TODO: docs
        public Type Type { get; }

        public string Method { get; }
        
        public ILProcessorAttribute(Type type)
        {
            Type = type;
        }

        public ILProcessorAttribute(Type type, string method)
        {
            Method = method;
        }
    }
}
