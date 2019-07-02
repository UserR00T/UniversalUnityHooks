using System;
using UniversalUnityHooks;

namespace TestProject
{
#pragma warning disable RCS1163, IDE0060, RCS1102
    public class Core
    {
		[Hook("Injected.Method3")]
        public static bool InjectedMethod_M(Injected instance)
        {
			return true;
		}

		[Hook("Injected.Method2")]
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
#pragma warning restore RCS1102, IDE0060, RCS1163
}
