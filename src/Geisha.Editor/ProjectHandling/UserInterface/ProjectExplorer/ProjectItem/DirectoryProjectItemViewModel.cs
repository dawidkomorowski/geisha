using System;
using Geisha.Editor.Core.ViewModels.Infrastructure;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem
{
    public class DirectoryProjectItemViewModel : ProjectItemViewModel
    {
        private readonly IProjectItemViewModelFactory _factory;
        private readonly IProjectItemObsolete _projectItem;
        private readonly IWindow _window;

        public DirectoryProjectItemViewModel(IProjectItemObsolete projectItem, IProjectItemViewModelFactory factory,
            IAddContextMenuItemFactory addContextMenuItemFactory, IWindow window) : base(projectItem.Name)
        {
            _projectItem = projectItem;
            _factory = factory;
            _window = window;

            UpdateItems(_factory.Create(_projectItem.ProjectItems, _window));

            ContextMenuItems.Add(addContextMenuItemFactory.Create(projectItem, window));

            projectItem.ProjectItemsChanged += ProjectItemOnProjectItemsChanged;
        }

        private void ProjectItemOnProjectItemsChanged(object sender, EventArgs eventArgs)
        {
            UpdateItems(_factory.Create(_projectItem.ProjectItems, _window));
        }
    }
}