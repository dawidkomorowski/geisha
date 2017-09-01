namespace Geisha.Editor.Core.Infrastructure
{
    public interface IRequestFilePathService
    {
        string RequestFilePath();
        string RequestFilePath(string fileTypeDisplayName, string extensionFilter);
        string RequestDirectoryPath();
    }
}