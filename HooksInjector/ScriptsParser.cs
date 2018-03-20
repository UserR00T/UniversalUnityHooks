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
        public ParsedAccessModifier[] GetAccessModifiers(string scriptFile)
        {
            if (!File.Exists(scriptFile))
            {
                Console.WriteLine(scriptFile + " doesn't exist!");
                Console.Read();
                return null;
            }

            string[] scriptLines = File.ReadAllLines(scriptFile);

            List<ParsedAccessModifier> ChangedAccessModifiers = new List<ParsedAccessModifier>();
            for (int i = 0; i < scriptLines.Length; i++)
            {
                string line = scriptLines[i];
                if (line.Contains(hookName))
                {
                    string ToAccessModifier = Regex.Match(line, "\"([^\"]*)\"").Groups[1].Value;
                    string AccessModifierField = Regex.Match(line, "\"([^\"]*)\"").Groups[2].Value;
                    if (AccessModifierField.Length < 1 || ToAccessModifier.Length < 1)
                    {
                        Console.WriteLine("HooksInjector: ERROR: " + scriptFile + " Contains incomplete Access Modifier or field on line: " + i);
                        Console.Read();
                        return null;
                    }
                    Console.WriteLine($"Changed {ToAccessModifier} to {AccessModifierField}");
                    ChangedAccessModifiers.Add(new ParsedAccessModifier
                    {
                        ToAccessModifier = ToAccessModifier,
                        AccessModifierField = AccessModifierField
                    });
                }
            }

            return ChangedAccessModifiers.ToArray();

        }
    }
}
