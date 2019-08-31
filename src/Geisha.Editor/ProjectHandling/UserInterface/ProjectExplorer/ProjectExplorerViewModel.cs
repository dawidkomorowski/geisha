﻿using System;
using System.Collections.ObjectModel;
using Geisha.Editor.Core.ViewModels.Infrastructure;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer
{
    public class ProjectExplorerViewModel : ViewModel, IWindowContext
    {
        private readonly IProjectItemViewModelFactory _projectItemViewModelFactory;
        private readonly IProjectServiceObsolete _projectService;

        public ProjectExplorerViewModel(IProjectItemViewModelFactory projectItemViewModelFactory, IProjectServiceObsolete projectService)
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