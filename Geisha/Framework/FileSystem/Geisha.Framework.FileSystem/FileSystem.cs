using System.ComponentModel.Composition;
using System.IO;

namespace Geisha.Framework.FileSystem
{
    public interface IFileSystem
    {
        string ReadAllTextFromFile(string path);
        void WriteAllTextToFile(string path, string contents);
    }

    [Export(typeof(IFileSystem))]
    internal class FileSystem : IFileSystem
    {
        public string ReadAllTextFromFile(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteAllTextToFile(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }
    }
}