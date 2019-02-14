namespace Geisha.Framework.FileSystem
{
    // TODO Add xml documentation.
    public interface IFile
    {
        string Name { get; }
        string Extension { get; }
        string Path { get; }

        string ReadAllText();
    }
}