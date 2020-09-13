using System.IO;
using CliFx.Exceptions;

namespace UniversalUnityHooks.Core
{
    public static class CliAssert
    {
        public static void Fail(string message)
        {
            throw new CommandException($"Assert failed: {message}");
        }

        public static void IsRequired(object obj, string name)
        {
            if (obj != null)
            {
                return;
            }
            Fail($"'{name}' is a required field, but was null.");
        }

        public static void IsDirectory(FileInfo file)
        {
            var attr = File.GetAttributes(file.FullName);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                return;
            }
            Fail($"Assert failed: {file.FullName} is not a directory.");
        }

        public static void IsNotDirectory(FileInfo file)
        {
            var attr = File.GetAttributes(file.FullName);
            if ((attr & FileAttributes.Directory) != FileAttributes.Directory)
            {
                return;
            }
            Fail($"Assert failed: {file.FullName} is a directory.");
        }

        public static void HasExtension(FileInfo file, string extension)
        {
            if (file.Extension?.ToLowerInvariant() == extension)
            {
                return;
            }
            Fail($"Assert failed: Expected extension '{extension}' for file '{file.FullName}', but got '{file.Extension}'.");
        }
    }
}
