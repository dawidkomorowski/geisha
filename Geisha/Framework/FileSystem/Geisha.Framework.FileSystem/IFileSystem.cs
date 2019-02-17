using System.IO;

namespace Geisha.Framework.FileSystem
{
    // TODO Add xml documentation.
    public interface IFileSystem
    {
        string ReadAllTextFromFile(string path);
        void WriteAllTextToFile(string path, string contents);
        Stream OpenFileStreamForReading(string path);
        IFile GetFile(string filePath);
        IDirectory GetDirectory(string directoryPath);
    }
}