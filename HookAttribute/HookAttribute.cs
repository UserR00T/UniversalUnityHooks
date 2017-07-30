using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HookAttribute : Attribute
{
	private string _fullName;
	public HookAttribute(string fullName, bool end = false)
	{
		_fullName = fullName;
	}
}