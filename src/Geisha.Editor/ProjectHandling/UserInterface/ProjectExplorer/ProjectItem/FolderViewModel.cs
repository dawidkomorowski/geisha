using System;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem
{
    public class FolderViewModel : ProjectExplorerItemViewModel
    {
        private readonly IProjectExplorerItemViewModelFactory _factory;
        private readonly IProjectFolder _folder;

        public FolderViewModel(IProjectFolder folder, IProjectExplorerItemViewModelFactory factory,
            IAddContextMenuItemFactory addContextMenuItemFactory) : base(folder.Name)
        {
            _folder = folder;
            _factory = factory;

            UpdateItems(_factory.Create(_folder.Folders, _folder.Files));

            ContextMenuItems.Add(addContextMenuItemFactory.Create(folder));

            folder.FolderAdded += OnFolderAdded;
        }

        private void OnFolderAdded(object sender, EventArgs eventArgs)
        {
            UpdateItems(_factory.Create(_folder.Folders, _folder.Files));
        }
    }
}