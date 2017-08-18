namespace Geisha.Editor.Core.Infrastructure
{
    public interface IRequestFilePathService
    {
        string RequestFilePath();
        string RequestFilePath(string rawDisplayName, string extensionList);
        string RequestDirectoryPath();
    }
}