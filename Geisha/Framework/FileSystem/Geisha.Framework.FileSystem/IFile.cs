namespace Geisha.Framework.FileSystem
{
    public interface IFile
    {
        string Name { get; }
        string Extension { get; }
        string Path { get; }

        string ReadAllText();
    }
}