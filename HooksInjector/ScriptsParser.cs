using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HooksInjector
{
	/// <summary>
	/// Responsible for finding hooks in scripts
	/// </summary>
	class ScriptsParser
	{
		private const string HOOK_ATTRIBUTE_NAME = "Hook";

		public struct ParsedHook
		{
			/// <summary>
			/// Method fullName path to the method that is to be hooked
			/// </summary>
			public string fullName;
			/// <summary>
			/// Wether method returns boolean, which means it can block the execution or not
			/// </summary>
			public bool canBlock;

			public bool hookEnd;
		}

		/// <summary>
		/// Parses a script file and finds all hook methods inside it
		/// </summary>
		/// <param name="scriptFile">File path to the script that is getting parsed</param>
		/// <returns></returns>
		public ParsedHook[] GetHooks(string scriptFile)
		{
			if(!File.Exists(scriptFile))
			{
				Console.WriteLine(scriptFile + " doesn't exist!");
				Console.ReadLine();
				return null;
			}

			string[] scriptLines = File.ReadAllLines(scriptFile);

			List<ParsedHook> hooks = new List<ParsedHook>();
			for(int i = 0; i < scriptLines.Length; i++)
			{
				string line = scriptLines[i];

				if(line.Contains(HOOK_ATTRIBUTE_NAME))
				{
					string methodFullName = Regex.Match(line, "\"([^\"]*)\"").Groups[1].Value;
					if(methodFullName.Length < 1)
					{
						Console.WriteLine(scriptFile + " Contains hook without full name at line: " + i);
						Console.ReadLine();
						return null;
					}

					bool methodCanBlock = scriptLines[i + 1].IndexOf(" void ") > -1 ? false : true;

					Console.WriteLine(methodFullName + " can block: " + methodCanBlock);

					bool hookEnd = false;
					if (line.Contains("true"))
						hookEnd = true;

					hooks.Add(new ParsedHook
					{
						fullName = methodFullName,
						canBlock = methodCanBlock,
						hookEnd = hookEnd
					});					
				}
			}

			return hooks.ToArray();
		}
	}
}
