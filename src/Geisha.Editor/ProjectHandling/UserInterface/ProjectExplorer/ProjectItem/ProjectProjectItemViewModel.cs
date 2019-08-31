using System;
using Geisha.Editor.Core.ViewModels.Infrastructure;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem
{
    public class ProjectProjectItemViewModel : ProjectItemViewModel
    {
        private readonly IProjectItemViewModelFactory _factory;
        private readonly IProjectObsolete _project;
        private readonly IWindow _window;

        public ProjectProjectItemViewModel(IProjectObsolete project, IProjectItemViewModelFactory factory,
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