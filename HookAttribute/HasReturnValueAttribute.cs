using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HookAttribute
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class HasReturnValueAttribute : Attribute
	{
		public HasReturnValueAttribute()
		{
		}
	}
}
