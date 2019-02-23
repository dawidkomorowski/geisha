namespace Geisha.Framework.FileSystem
{
    // TODO Add xml documentation.
    public interface IFileSystem
    {
        IFile CreateFile(string filePath);
        IFile GetFile(string filePath);
        IDirectory GetDirectory(string directoryPath);
    }
}