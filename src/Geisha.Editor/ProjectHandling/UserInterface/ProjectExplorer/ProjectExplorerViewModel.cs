using System;
using System.Collections.ObjectModel;
using Geisha.Editor.Core.ViewModels;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer
{
    public class ProjectExplorerViewModel : ViewModel, IWindowContext
    {
        private readonly IProjectExplorerItemViewModelFactory _projectExplorerItemViewModelFactory;
        private readonly IProjectService _projectService;

        public ProjectExplorerViewModel(IProjectExplorerItemViewModelFactory projectExplorerItemViewModelFactory, IProjectService projectService)
        {
            _projectExplorerItemViewModelFactory = projectExplorerItemViewModelFactory;
            _projectService = projectService;

            _projectService.CurrentProjectChanged += OnCurrentProjectChanged;
        }

        public ObservableCollection<ProjectExplorerItemViewModel> Items { get; } = new ObservableCollection<ProjectExplorerItemViewModel>();

        public IWindow Window { get; set; }

        private void OnCurrentProjectChanged(object sender, EventArgs eventArgs)
        {
            Items.Clear();

            if (_projectService.ProjectIsOpen)
            {
                Items.Add(_projectExplorerItemViewModelFactory.Create(_projectService.CurrentProject, Window));
            }
        }
    }
}