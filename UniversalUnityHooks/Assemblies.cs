using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static UniversalUnityHooks.Util;
using HookAttribute;
using UniversalUnityHooks.Attributes;

namespace UniversalUnityHooks
{
    public static class Assemblies
    {
        public static void GetAllAssembliesInsideDirectory(string directory)
        {
            try
            {
                foreach (var file in Directory.GetFiles(directory, "*.dll"))
                {
                    ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Wait, $"Loading in assembly {Path.GetFileName(file)}..");
                    var timer = new OperationTimer();
                    try
                    {
                        var assembly = AssemblyDefinition.ReadAssembly(Path.GetFullPath(file));
                        var allAttributes = AttributesHelper.GetAllAttributesInAssembly(assembly);
						var filteredAttributes = AttributesHelper.FindAndInvokeAllAttributes(allAttributes, timer);
						foreach (var keyValuePair in filteredAttributes)
							Program.Attributes.Add(keyValuePair.Key, keyValuePair.Value);
						timer.Stop();
                        ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, $"Loaded in assembly and {filteredAttributes.Sum(x=>x.Value.Count)} attribute(s) in {timer.GetElapsedMs}ms\r\n");
                    }
                    catch (Exception ex)
                    {
						ConsoleHelper.WriteError(ex.InnerException.StackTrace);
						timer.Stop();
                        ConsoleHelper.WriteError($"Could not load in assembly after {timer.GetElapsedMs}ms.");
                        ConsoleHelper.WriteError($"Message: {ex.Message}");
                        ConsoleHelper.WriteError(ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"{ex.Message}\r\nStackTrace:{ex.StackTrace}");
            }
        }
    }
}
