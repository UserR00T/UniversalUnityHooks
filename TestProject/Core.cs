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
		[HookAttribute.Hook("Injected.Method3")]
		public static bool InjectedMethod_M(Injected instance)
		{
			return true;
		}
		[HookAttribute.Hook("Injected.Method2")]
		public static void InjectedMethod2_M(Injected instance)
		{
			Console.WriteLine("InjectedMethod2_M called");
		}


		[AddMethod(nameof(Injected), "TestMethod")]
		public static void OnTestMethod(Injected instance)
		{
			Console.WriteLine("OnTestMethod Called");
		}
		[AddMethod(nameof(StaticInjected), "StaticTestMethod")]
		public static void OnStaticTestMethod()
		{
			Console.WriteLine("OnStaticTestMethod Called");
		}
	}
}
