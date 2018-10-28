using System;
using Geisha.Editor.Core.Models.Domain.ProjectHandling;
using Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer.ProjectItem.ContextMenuItems.Add;
using Geisha.Editor.Core.ViewModels.Infrastructure;

namespace Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer.ProjectItem
{
    public class DirectoryProjectItemViewModel : ProjectItemViewModel
    {
        private readonly IProjectItemViewModelFactory _factory;
        private readonly IProjectItem _projectItem;
        private readonly IWindow _window;

        public DirectoryProjectItemViewModel(IProjectItem projectItem, IProjectItemViewModelFactory factory,
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