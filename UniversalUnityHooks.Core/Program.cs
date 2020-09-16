using CliFx;
using System.Threading.Tasks;

namespace UniversalUnityHooks.Core
{
    public static class Program
    {
        public static async Task<int> Main() =>
            await new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                .Build()
                .RunAsync();
    }
}
