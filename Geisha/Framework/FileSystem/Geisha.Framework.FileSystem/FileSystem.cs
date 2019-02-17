using System.IO;

namespace Geisha.Framework.FileSystem
{
    internal sealed class FileSystem : IFileSystem
    {
        public string ReadAllTextFromFile(string path)
        {
            return File.ReadAllText(path);
        }

        public void WriteAllTextToFile(string path, string contents)
        {
            File.WriteAllText(path, contents);
        }

        public Stream OpenFileStreamForReading(string path)
        {
            return new FileStream(path, FileMode.Open);
        }

        public IFile GetFile(string filePath)
        {
            throw new System.NotImplementedException();
        }

        public IDirectory GetDirectory(string directoryPath)
        {
            throw new System.NotImplementedException();
        }
    }
}