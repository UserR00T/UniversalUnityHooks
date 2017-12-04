using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CommandLine;

namespace HooksInjector
{
    public class ScriptsParser
    {
        const string hookName = "Hook";
        public struct ParsedHook
        {
            public string fullName;
            public bool canBlock;
            public bool hookEnd;
        }

        public ParsedHook[] GetHooks(string scriptFile) {
            if (!File.Exists(scriptFile)) {
                Console.WriteLine(scriptFile + " doesn't exist!");
                Console.Read();
                return null;
            }

            string[] scriptLines = File.ReadAllLines(scriptFile);

            List<ParsedHook> hooks = new List<ParsedHook>();
            for (int i = 0; i < scriptLines.Length; i++) {
                string line = scriptLines[i];
                if (line.Contains(hookName)) {
                    string methodName = Regex.Match(line, "\"([^\"]*)\"").Groups[1].Value;
                    if (methodName.Length < 1) {
                        Console.WriteLine("HooksInjector: ERROR: " + scriptFile + " Contains incomplete hook on line: " + i);
                        Console.Read();
                        return null;
                    }

                    bool methodCanBlock = scriptLines[i + 1].IndexOf(" void ") <= -1;

                    Console.WriteLine(methodName + " Can Block: " + methodCanBlock);

                    bool hookEnd = false || line.Contains("true");
                    hooks.Add(new ParsedHook {
                        fullName = methodName,
                        canBlock = methodCanBlock,
                        hookEnd = hookEnd

                    });
                }
            }

            return hooks.ToArray();

        }
    }
}
