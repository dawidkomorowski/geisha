using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Geisha.Editor.Core.Infrastructure;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Geisha.Editor.Core.Views.Infrastructure
{
    [Export(typeof(IRequestFilePathService))]
    internal class OpenFileDialogRequestFilePathService : IRequestFilePathService
    {
        public string RequestFilePath()
        {
            return GetPath(dialog => dialog.IsFolderPicker = false);
        }

        public string RequestFilePath(string fileTypeDisplayName, string extensionFilter)
        {
            return GetPath(dialog =>
            {
                dialog.IsFolderPicker = false;
                dialog.Filters.Add(new CommonFileDialogFilter(fileTypeDisplayName, extensionFilter));
            });
        }

        public string RequestDirectoryPath()
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