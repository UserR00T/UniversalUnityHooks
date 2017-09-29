using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HooksInjector
{
	class HookAttribute : Attribute
	{
		private string hookName;
		public HookAttribute(string fullName, bool end = false)
		{
			hookName = fullName;
		}

	}
}
