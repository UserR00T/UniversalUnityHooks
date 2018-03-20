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
        private const string HookName = "Hook";
        private const string CamAttributeName = "ChangeAccessModifier";
        public struct ParsedHook
        {
            public string FullName;
            public bool CanBlock;
            public bool HookEnd;
        }
        public struct ParsedAccessModifier
        {
            public string ToAccessModifier;
            public string AccessModifierField;
        }

        public ParsedHook[] GetHooks(string scriptFile) {
            if (!File.Exists(scriptFile)) {
                Console.WriteLine(scriptFile + " doesn't exist!");
                Console.Read();
                return null;
            }

            string[] scriptLines = File.ReadAllLines(scriptFile);

            List<ParsedHook> hooks = new List<ParsedHook>();
            for (var i = 0; i < scriptLines.Length; i++) {
                string line = scriptLines[i];
                if (line.Contains(HookName)) {
                    string methodName = Regex.Match(line, "\"([^\"]*)\"").Groups[1].Value;
                    if (methodName.Length < 1) {
                        Console.WriteLine("HooksInjector: ERROR: " + scriptFile + " Contains incomplete hook on line: " + i);
                        Console.Read();
                        return null;
                    }
                    bool methodCanBlock = scriptLines[i + 1].IndexOf(" void ", StringComparison.Ordinal) <= -1;
                    Console.WriteLine(methodName + " Can Block: " + methodCanBlock);
                    bool hookEnd = false || line.Contains("true");
                    hooks.Add(new ParsedHook {
                        FullName = methodName,
                        CanBlock = methodCanBlock,
                        HookEnd = hookEnd
                    });
                }
            }
            return hooks.ToArray();
        }
        public static ParsedAccessModifier[] GetAccessModifiers(string scriptFile)
        {
            if (!File.Exists(scriptFile))
            {
                Console.WriteLine(scriptFile + " doesn't exist!");
                Console.Read();
                return null;
            }
            string[] scriptLines = File.ReadAllLines(scriptFile);
            List<ParsedAccessModifier> changedAccessModifiers = new List<ParsedAccessModifier>();
            for (var i = 0; i < scriptLines.Length; i++)
            {
                string line = scriptLines[i];
                if (line.Contains(CamAttributeName))
                {
                    string toAccessModifier = Regex.Match(line, "\"([^\"]*)\"").Groups[1].Value;
                    string accessModifierField = Regex.Match(line, "\"([^\"]*)\"").Groups[2].Value;
                    if (accessModifierField.Length < 1 || toAccessModifier.Length < 1)
                    {
                        Console.WriteLine("HooksInjector: ERROR: " + scriptFile + " Contains incomplete Access Modifier or field on line: " + i);
                        Console.Read();
                        return null;
                    }
                    Console.WriteLine($"Changed {toAccessModifier} to {accessModifierField}");
                    changedAccessModifiers.Add(new ParsedAccessModifier
                    {
                        ToAccessModifier = toAccessModifier,
                        AccessModifierField = accessModifierField
                    });
                }
            }

            return changedAccessModifiers.ToArray();

        }
    }
}
