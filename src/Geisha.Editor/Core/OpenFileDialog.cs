using System;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Geisha.Editor.Core
{
    public interface IContinueWith
    {
        OpenFileDialogEventArgs AndContinueWith(Action<string?> actionOnPath);
    }

    public sealed class OpenFileDialogEventArgs : EventArgs
    {
        public static IContinueWith AskForFilePath(string fileTypeDisplayName, string extensionFilter)
        {
            return new ContinueWith(false, fileTypeDisplayName, extensionFilter);
        }

        public static IContinueWith AskForDirectoryPath()
        {
            return new ContinueWith(true, string.Empty, string.Empty);
        }

        private OpenFileDialogEventArgs(bool isFolderPicker, string fileTypeDisplayName, string extensionFilter, Action<string?> continuation)
        {
            IsFolderPicker = isFolderPicker;
            FileTypeDisplayName = fileTypeDisplayName;
            ExtensionFilter = extensionFilter;
            Continuation = continuation;
        }

        public bool IsFolderPicker { get; }
        public string FileTypeDisplayName { get; }
        public string ExtensionFilter { get; }
        public Action<string?> Continuation { get; }

        private sealed class ContinueWith : IContinueWith
        {
            private readonly bool _isFolderPicker;
            private readonly string _fileTypeDisplayName;
            private readonly string _extensionFilter;

            public ContinueWith(bool isFolderPicker, string fileTypeDisplayName, string extensionFilter)
            {
                _isFolderPicker = isFolderPicker;
                _fileTypeDisplayName = fileTypeDisplayName;
                _extensionFilter = extensionFilter;
            }

            public OpenFileDialogEventArgs AndContinueWith(Action<string?> actionOnPath)
            {
                return new OpenFileDialogEventArgs(_isFolderPicker, _fileTypeDisplayName, _extensionFilter, actionOnPath);
            }
        }
    }

    internal static class OpenFileDialog
    {
        private static string? GetPath(Action<CommonOpenFileDialog> configAction, Window owner)
        {
            string? filePath = null;

            using var commonOpenFileDialog = new CommonOpenFileDialog();
            configAction(commonOpenFileDialog);

            var commonFileDialogResult = commonOpenFileDialog.ShowDialog(owner);
            if (commonFileDialogResult == CommonFileDialogResult.Ok)
            {
                filePath = commonOpenFileDialog.FileName;
            }

            return filePath;
        }

        public static void HandleEvent(OpenFileDialogEventArgs args, Window owner)
        {
            var path = GetPath(dialog =>
            {
                dialog.IsFolderPicker = args.IsFolderPicker;
                if (!args.IsFolderPicker) dialog.Filters.Add(new CommonFileDialogFilter(args.FileTypeDisplayName, args.ExtensionFilter));
            }, owner);

            args.Continuation(path);
        }
    }
}