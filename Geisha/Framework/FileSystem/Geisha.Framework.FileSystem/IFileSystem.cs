using System.IO;

namespace Geisha.Framework.FileSystem
{
    // TODO Add xml documentation.
    public interface IFileSystem
    {
        // TODO Replace with GetFile().OpenRead()
        Stream OpenFileStreamForReading(string path);
        IFile CreateFile(string filePath);
        IFile GetFile(string filePath);
        IDirectory GetDirectory(string directoryPath);
    }
}