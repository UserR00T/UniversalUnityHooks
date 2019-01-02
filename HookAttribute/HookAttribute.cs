using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HookAttribute
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class HookAttribute : Attribute
    {
        bool AddToEnd { get; set; }
        string FullName { get; set; }
        public HookAttribute(string fullName, bool addToEnd = false)
        {
            FullName = fullName;
            AddToEnd = addToEnd;
        }
        public string GetName() => FullName;
        public bool PlaceAtEnd() => AddToEnd;
    }
}
