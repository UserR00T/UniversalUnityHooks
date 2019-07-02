using System;

namespace UniversalUnityHooks
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class AddMethodAttribute : Attribute
	{
        private string FullName { get; }

        private string MethodName { get; }

        public AddMethodAttribute(string fullName, string methodName)
		{
			FullName = fullName;
			MethodName = methodName;
		}
	}
}
