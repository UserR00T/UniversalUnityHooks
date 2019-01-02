using HookAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestProject
{
	public class Core
	{
		public void InjectedMethod()
		{
			Console.WriteLine("Magic v1");
		}
		public void InjectedMethod2()
		{
			Console.WriteLine("Magic v2");
		}




		[HookAttribute.Hook("Core.InjectedMethod")]
		public static bool InjectedMethod_M(Core instance)
		{
			return true;
		}
		[HookAttribute.Hook("Core.InjectedMethod2")]
		public static void InjectedMethod2_M(Core instance)
		{
			Console.WriteLine("InjectedMethod2_M called");
		}
		[AddMethod(nameof(Core), "TestMethod")]
		public static void OnTestMethod(Core instance)
		{
			Console.WriteLine("OnTestMethod Called");
		}
	}
}
