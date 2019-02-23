using System.IO;

namespace Geisha.Framework.FileSystem
{
    public sealed class FileSystem : IFileSystem
    {
        public void WriteAllTextToFile(string path, string contents)
        {
            System.IO.File.WriteAllText(path, contents);
        }

        public Stream OpenFileStreamForReading(string path)
        {
            return new FileStream(path, FileMode.Open);
        }

        public IFile CreateFile(string filePath)
        {
            System.IO.File.WriteAllText(filePath, string.Empty);
            return new File(filePath);
        }

        public IFile GetFile(string filePath)
        {
            return new File(filePath);
        }

        public IDirectory GetDirectory(string directoryPath)
        {
            return new Directory(directoryPath);
        }
    }
}