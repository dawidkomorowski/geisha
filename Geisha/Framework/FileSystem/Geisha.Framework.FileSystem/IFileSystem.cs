namespace Geisha.Framework.FileSystem
{
    public interface IFileSystem
    {
        string ReadFileAllText(string path);
    }
}