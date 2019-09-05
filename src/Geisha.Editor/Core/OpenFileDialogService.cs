using System;
using System.Linq;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Geisha.Editor.Core
{
    public interface IOpenFileDialogService
    {
        string AskForFilePath();
        string AskForFilePath(string fileTypeDisplayName, string extensionFilter);
        string AskForDirectoryPath();
    }

    internal sealed class OpenFileDialogService : IOpenFileDialogService
    {
        public string AskForFilePath()
        {
            return GetPath(dialog => dialog.IsFolderPicker = false);
        }

        public string AskForFilePath(string fileTypeDisplayName, string extensionFilter)
        {
            return GetPath(dialog =>
            {
                dialog.IsFolderPicker = false;
                dialog.Filters.Add(new CommonFileDialogFilter(fileTypeDisplayName, extensionFilter));
            });
        }

        public string AskForDirectoryPath()
        {
            return GetPath(dialog => dialog.IsFolderPicker = true);
        }

        private string GetPath(Action<CommonOpenFileDialog> configAction)
        {
            string filePath = null;
            var owner = GetOwnerWindow();

            using (var commonOpenFileDialog = new CommonOpenFileDialog())
            {
                configAction(commonOpenFileDialog);

                var commonFileDialogResult = commonOpenFileDialog.ShowDialog(owner);
                if (commonFileDialogResult == CommonFileDialogResult.Ok)
                {
                    filePath = commonOpenFileDialog.FileName;
                }
            }

            return filePath;
        }

        private static Window GetOwnerWindow()
        {
            return Application.Current.Windows.OfType<Window>().Single(w => w.IsActive);
        }
    }
}