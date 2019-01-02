using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HookAttribute
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class AddMethodAttribute : Attribute
	{
		string FullName { get; set; }
		string MethodName { get; set; }
		public AddMethodAttribute(string fullName, string methodName)
		{
			FullName = fullName;
			MethodName = methodName;
		}
		public string GetName() => FullName;
		public string GetMethodName() => MethodName;
	}
}
