using System;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem
{
    internal class ProjectRootViewModel : ProjectExplorerItemViewModel
    {
        private readonly IProjectExplorerItemViewModelFactory _factory;
        private readonly IProject _project;

        public ProjectRootViewModel(IProject project, IProjectExplorerItemViewModelFactory factory,
            IAddContextMenuItemFactory addContextMenuItemFactory) : base(project.Name)
        {
            _project = project;
            _factory = factory;

            UpdateItems(_factory.Create(project.Folders, project.Files));

            ContextMenuItems.Add(addContextMenuItemFactory.Create(_project));

            project.FolderAdded += OnProjectItemsChanged;
            project.FileAdded += OnProjectItemsChanged;
        }

        private void OnProjectItemsChanged(object sender, EventArgs eventArgs)
        {
            UpdateItems(_factory.Create(_project.Folders, _project.Files));
        }
    }
}