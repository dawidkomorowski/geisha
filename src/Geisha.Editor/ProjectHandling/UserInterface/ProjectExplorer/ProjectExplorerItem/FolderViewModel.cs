using System;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem.ContextMenuItems.Add;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectExplorerItem
{
    internal sealed class FolderViewModel : ProjectExplorerItemViewModel
    {
        private readonly IProjectExplorerItemViewModelFactory _factory;
        private readonly IProjectFolder _folder;

        public FolderViewModel(IProjectFolder folder, IProjectExplorerItemViewModelFactory factory,
            IAddContextMenuItemFactory addContextMenuItemFactory) : base(folder.FolderName)
        {
            _folder = folder;
            _factory = factory;

            UpdateItems(_factory.Create(_folder.Folders, _folder.Files));

            ContextMenuItems.Add(addContextMenuItemFactory.Create(folder));

            folder.FolderAdded += OnFolderItemsChanged;
            folder.FileAdded += OnFolderItemsChanged;
        }

        private void OnFolderItemsChanged(object? sender, EventArgs eventArgs)
        {
            UpdateItems(_factory.Create(_folder.Folders, _folder.Files));
        }
    }
}