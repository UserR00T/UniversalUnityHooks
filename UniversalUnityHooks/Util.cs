using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniversalUnityHooks
{
    public static partial class Util
    {
        public static string GetManagedDirectory()
        {
            foreach (string directory in Directory.GetDirectories(Directory.GetCurrentDirectory()))
                if (directory.EndsWith("_Data", StringComparison.CurrentCulture))
                    return Path.Combine(directory, "Managed\\");
            ConsoleHelper.WriteMessage(ConsoleHelper.MessageType.Error, "Cannot find Managed folder. Please make sure that you've put the .exe in the root folder of the game. Press any key to exit the program.");
            return null;
        }
        public static void CreateFiles()
        {
            if (!Directory.Exists(Program.pluginsFolder))
                Directory.CreateDirectory(Program.pluginsFolder);

        }
    }
}
