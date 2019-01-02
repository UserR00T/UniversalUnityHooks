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
                        var attributes = AttributesHelper.GetAllAttributes(assembly);
						// Improve
						var hookAttributes = new HookAttributes(attributes, timer);
						if (hookAttributes.AddAllFound() == AttributesHelper.AddAttributesResponse.Error)
							continue;
						Program.HookAttributes.Add(hookAttributes.Attributes);
						var addMethodAttributes = new Attributes.AddMethodAttribute(attributes, timer);
						if (addMethodAttributes.AddAllFound() == AttributesHelper.AddAttributesResponse.Info)
							continue;
						Program.AddMethodAttributes.Add(addMethodAttributes.Attributes);
						timer.Stop();
                        ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, $"Loaded in assembly and {hookAttributes.Count + addMethodAttributes.Count} attribute(s) in {timer.GetElapsedMs}ms\r\n");
                    }
                    catch (Exception ex)
                    {
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
