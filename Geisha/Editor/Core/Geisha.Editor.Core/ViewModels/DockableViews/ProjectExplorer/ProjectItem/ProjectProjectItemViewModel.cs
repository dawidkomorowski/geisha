using System;
using Geisha.Editor.Core.Models.Domain.ProjectHandling;
using Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer.ProjectItem.ContextMenuItems.Add;
using Geisha.Editor.Core.ViewModels.Infrastructure;

namespace Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer.ProjectItem
{
    public class ProjectProjectItemViewModel : ProjectItemViewModel
    {
        private readonly IProjectItemViewModelFactory _factory;
        private readonly IProject _project;
        private readonly IWindow _window;

        public ProjectProjectItemViewModel(IProject project, IProjectItemViewModelFactory factory,
            IAddContextMenuItemFactory addContextMenuItemFactory, IWindow window) : base(project.Name)
        {
            _project = project;
            _factory = factory;
            _window = window;

            UpdateItems(_factory.Create(project.ProjectItems, window));

            ContextMenuItems.Add(addContextMenuItemFactory.Create(null, window));

            project.ProjectItemsChanged += ProjectOnProjectItemsChanged;
        }

        private void ProjectOnProjectItemsChanged(object sender, EventArgs eventArgs)
        {
            UpdateItems(_factory.Create(_project.ProjectItems, _window));
        }
    }
}