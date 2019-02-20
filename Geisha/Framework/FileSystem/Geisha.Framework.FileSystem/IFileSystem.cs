using System.IO;

namespace Geisha.Framework.FileSystem
{
    // TODO Add xml documentation.
    public interface IFileSystem
    {
        // TODO Replace with GetFile().ReadAllText()
        string ReadAllTextFromFile(string path);
        // TODO Replace with CreateFile().WriteAllText()
        void WriteAllTextToFile(string path, string contents);
        // TODO Replace with GetFile().OpenRead()
        Stream OpenFileStreamForReading(string path);
        IFile CreateFile(string filePath);
        IFile GetFile(string filePath);
        IDirectory GetDirectory(string directoryPath);
    }
}