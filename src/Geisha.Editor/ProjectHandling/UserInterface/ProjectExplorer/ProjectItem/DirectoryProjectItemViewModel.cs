using System;
using Geisha.Editor.Core.ViewModels.Infrastructure;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem
{
    public class DirectoryProjectItemViewModel : ProjectItemViewModel
    {
        private readonly IProjectItemViewModelFactory _factory;
        private readonly IProjectFolder _folder;
        private readonly IWindow _window;

        public DirectoryProjectItemViewModel(IProjectFolder folder, IProjectItemViewModelFactory factory,
            IAddContextMenuItemFactory addContextMenuItemFactory, IWindow window) : base(folder.Name)
        {
            _folder = folder;
            _factory = factory;
            _window = window;

            UpdateItems(_factory.Create(_folder.Folders, _folder.Files, _window));

            ContextMenuItems.Add(addContextMenuItemFactory.Create(folder, window));

            folder.FolderAdded += OnFolderAdded;
        }

        private void OnFolderAdded(object sender, EventArgs eventArgs)
        {
            UpdateItems(_factory.Create(_folder.Folders, _folder.Files, _window));
        }
    }
}