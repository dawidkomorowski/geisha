using System;
using System.Collections.ObjectModel;
using Geisha.Editor.Core.Models.Domain.ProjectHandling;
using Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer.ProjectItem;
using Geisha.Editor.Core.ViewModels.Infrastructure;

namespace Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer
{
    public class ProjectExplorerViewModel : ViewModel, IWindowContext
    {
        private readonly IProjectItemViewModelFactory _projectItemViewModelFactory;
        private readonly IProjectService _projectService;

        public ProjectExplorerViewModel(IProjectItemViewModelFactory projectItemViewModelFactory, IProjectService projectService)
        {
            _projectItemViewModelFactory = projectItemViewModelFactory;
            _projectService = projectService;

            _projectService.CurrentProjectChanged += ProjectServiceOnCurrentProjectChanged;
        }

        public ObservableCollection<ProjectItemViewModel> Items { get; } = new ObservableCollection<ProjectItemViewModel>();

        public IWindow Window { get; set; }

        private void ProjectServiceOnCurrentProjectChanged(object sender, EventArgs eventArgs)
        {
            if (_projectService.IsProjectOpen)
                Items.Add(_projectItemViewModelFactory.Create(_projectService.CurrentProject, Window));
            else
                Items.Clear();
        }
    }
}