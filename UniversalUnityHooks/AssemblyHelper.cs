using Mono.Cecil;
using System;
using System.IO;
using System.Linq;

namespace UniversalUnityHooks
{
    public class AssemblyHelper
    {
        public string AssemblyDirectory { get; }

        public AssemblyHelper(string directory)
        {
            AssemblyDirectory = directory;
        }

        public void FetchAndLoadAll()
        {
            try
            {
                foreach (var file in Directory.GetFiles(AssemblyDirectory, "*.dll"))
                {
                    Program.Chalker.WriteWait($"Loading in assembly {Path.GetFileName(file)}..");
                    var timer = new OperationTimer();
                    try
                    {
                        var assembly = AssemblyDefinition.ReadAssembly(Path.GetFullPath(file));
                        var allAttributes = Program.AttributesHelper.FindInAssembly(assembly);
						var filteredAttributes = Program.AttributesHelper.InstantiateAndInvoke(allAttributes, timer);
                        foreach (var kvp in filteredAttributes)
                        {
                            if (!Program.Attributes.ContainsKey(kvp.Key))
                            {
                                Program.Attributes.Add(kvp.Key, kvp.Value);
                            }
                            Program.Attributes[kvp.Key].AddRange(kvp.Value);
                        }
						timer.Stop();
                        Program.Chalker.WriteSuccess($"Loaded in assembly and {filteredAttributes.Sum(x=>x.Value.Count)} attribute(s) in {timer.GetElapsedMs}ms\r\n");
                    }
                    catch (Exception ex)
                    {
						timer.Stop();
                        Program.Chalker.WriteError($"Could not load in assembly after {timer.GetElapsedMs}ms.");
                        Program.Chalker.WriteError($"Message: {ex.Message}");
                        Program.Chalker.WriteError(ex.StackTrace);
                    }
                }
            }
            catch (Exception ex)
            {
                Program.Chalker.WriteError($"{ex.Message}\r\nStackTrace:{ex.StackTrace}");
            }
        }
    }
}
