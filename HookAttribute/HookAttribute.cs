using System;

namespace UUH2
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
