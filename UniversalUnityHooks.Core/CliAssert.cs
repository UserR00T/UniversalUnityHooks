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
            if (attr.HasFlag(FileAttributes.Directory))
            {
                return;
            }
            Fail($"{file.FullName} is not a directory.");
        }

        public static void IsFile(FileInfo file)
        {
            if (file.Exists)
            {
                return;
            }
            Fail($"{file.FullName} is not a file or does not exist.");
        }

        public static void HasExtension(FileInfo file, string extension)
        {
            if (file.Extension?.ToLowerInvariant() == extension)
            {
                return;
            }
            Fail($"Expected extension '{extension}' for file '{file.FullName}', but got '{file.Extension}'.");
        }
    }
}
