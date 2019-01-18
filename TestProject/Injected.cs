using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
	public class Injected
	{
		public void Method1()
		{
			Console.WriteLine("Magic v1");
		}
		public void Method2()
		{
			Console.WriteLine("Magic v2");
		}
		public bool Method3()
		{
			Console.WriteLine("Magic v3");
			return true;
		}
		public bool Method4()
		{
			Console.WriteLine("Magic v4");
			return false;
		}

		public static void StaticMethod1()
		{
			Console.WriteLine("Magic v1");
		}
		public static void StaticMethod2()
		{
			Console.WriteLine("Magic v2");
		}
		public static bool StaticMethod3()
		{
			Console.WriteLine("Magic v3");
			return true;
		}
		public static bool StaticMethod4()
		{
			Console.WriteLine("Magic v4");
			return false;
		}
	}
	public static class StaticInjected
	{

	}
}
