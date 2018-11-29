using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static UniversalUnityHooks.Util;

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
                        var attributes = Attributes.GetAllAttributes(assembly);
                        var hookAttributes = attributes.Where(x => x.attribute.AttributeType.Name == nameof(HookAttribute)).ToList();
                        if (hookAttributes == null || hookAttributes.Count == 0)
                        {
                            ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Warning, $"No hook attributes were found. Without hook attributes it cannot inject anything. ({timer.GetElapsedMs}ms)\r\n");
                            continue;
                        }
                        if (hookAttributes.Any(x=>!x.type.IsPublic))
                        {
                            ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"All types (classes that contain the method for example) must be public. ({timer.GetElapsedMs}ms)\r\n");
                            continue;
                        }
                        Program.hookAttributes.Add(hookAttributes);
                        timer.Stop();
                        ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Success, $"Loaded in assembly and {hookAttributes.Count} attribute(s) in {timer.GetElapsedMs}ms\r\n");
                    }
                    catch (Exception ex)
                    {
                        timer.Stop();
                        ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"Could not load in assembly after {timer.GetElapsedMs}ms.");
                        ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"Message: {ex.Message}");
                        ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, $"{ex.Message}\r\nStackTrace:{ex.StackTrace}");
            }
        }
    }
}
