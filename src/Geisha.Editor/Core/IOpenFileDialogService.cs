namespace Geisha.Editor.Core
{
    public interface IOpenFileDialogService
    {
        string AskForFilePath();
        string AskForFilePath(string fileTypeDisplayName, string extensionFilter);
        string AskForDirectoryPath();
    }
}