using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using Geisha.Editor.Core.Models.Domain.ProjectHandling;
using Geisha.Editor.Core.ViewModels.Infrastructure;

namespace Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer
{
    [Export(typeof(ProjectExplorerViewModel))]
    public class ProjectExplorerViewModel : ViewModel
    {
        private readonly IProjectService _projectService;

        public ObservableCollection<ProjectItemViewModel> Items { get; } = new ObservableCollection<ProjectItemViewModel>();

        [ImportingConstructor]
        public ProjectExplorerViewModel(IProjectService projectService)
        {
            _projectService = projectService;

            _projectService.CurrentProjectChanged += ProjectServiceOnCurrentProjectChanged;
        }

        private void ProjectServiceOnCurrentProjectChanged(object sender, EventArgs eventArgs)
        {
            if (_projectService.IsProjectOpen)
            {
                Items.Clear();

                var projectItemViewModels = MapFromProjectItems(_projectService.CurrentProject.ProjectItems);
                var currentProjectName = _projectService.CurrentProject.Name;

                Items.Add(new ProjectItemViewModel(currentProjectName, projectItemViewModels));
            }
            else
            {
                Items.Clear();
            }
        }

        private static IEnumerable<ProjectItemViewModel> MapFromProjectItems(IEnumerable<ProjectItem> projectItems)
        {
            return projectItems.OrderByDescending(pi => pi.Type).ThenBy(pi => pi.Name)
                .Select(pi => new ProjectItemViewModel(pi.Name, MapFromProjectItems(pi.ProjectItems)));
        }
    }
}