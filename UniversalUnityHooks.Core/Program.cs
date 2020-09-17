using CliFx;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace UniversalUnityHooks.Core
{
    public static class Program
    {
        public static string Version { get; } = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
   
        public static async Task<int> Main() =>
            await new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                .Build()
                .RunAsync();
    }
}
