using System.ComponentModel.Composition;
using System.IO;

namespace Geisha.Framework.FileSystem
{
    [Export(typeof(IFileSystem))]
    internal class FileSystem : IFileSystem
    {
        public string ReadFileAllText(string path)
        {
            return File.ReadAllText(path);
        }
    }
}