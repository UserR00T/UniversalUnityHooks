using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalUnityHooks
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class HookAttribute : Attribute
    {
        readonly bool addToEnd;
        readonly string fullName;
        public HookAttribute(string fullName, bool addToEnd = false)
        {
            this.fullName = fullName;
            this.addToEnd = addToEnd;
        }
        public string GetName() => fullName;
        public bool PlaceAtEnd() => addToEnd;
    }
}
