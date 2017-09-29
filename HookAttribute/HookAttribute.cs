using System;

namespace HooksInjector
{
	class HookAttribute : Attribute
	{
		private string hookName;
		public HookAttribute(string fullName)
		{
			hookName = fullName;
		}

	}
}
