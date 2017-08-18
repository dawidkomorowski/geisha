﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using Geisha.Editor.Core.Infrastructure;
using Geisha.Editor.Core.Models.Domain.ProjectHandling;
using Geisha.Editor.Core.Models.Persistence.ProjectHandling;
using Geisha.Editor.Core.ViewModels.Infrastructure;
using Geisha.Editor.Core.ViewModels.MainWindow.NewProjectDialog;

namespace Geisha.Editor.Core.ViewModels.MainWindow
{
    [Export]
    public class MainViewModel : ViewModel, IWindowContext
    {
        private readonly IVersionProvider _versionProvider;
        private readonly IRequestFilePathService _requestFilePathService;
        private readonly IProjectService _projectService;
        private readonly INewProjectDialogViewModelFactory _newProjectDialogViewModelFactory;

        private string _currentProjectName;
        private readonly RelayCommand _closeProjectCommand;

        private string ApplicationVersion => _versionProvider.GetCurrentVersion().ToString();

        private string CurrentProjectName
        {
            get { return _currentProjectName; }
            set { Set(ref _currentProjectName, value); }
        }

        public IWindow Window { get; set; }
        public ObservableCollection<DockableViewViewModel> DockableViewsViewModels { get; set; } = new ObservableCollection<DockableViewViewModel>();

        [DependsOnProperty(nameof(CurrentProjectName))]
        public string ApplicationTitle
        {
            get
            {
                var prefix = string.IsNullOrEmpty(CurrentProjectName) ? string.Empty : $"{CurrentProjectName} - ";
                return $"{prefix}Geisha Editor v{ApplicationVersion}";
            }
        }


        public ICommand NewProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand CloseProjectCommand => _closeProjectCommand;
        public ICommand ExitCommand { get; }


        [ImportingConstructor]
        public MainViewModel(IVersionProvider versionProvider, IRequestFilePathService requestFilePathService,
            IProjectService projectService, INewProjectDialogViewModelFactory newProjectDialogViewModelFactory,
            [ImportMany] IEnumerable<IDockableViewViewModelFactory> dockableViewViewModelFactories)
        {
            _versionProvider = versionProvider;
            _requestFilePathService = requestFilePathService;
            _projectService = projectService;
            _newProjectDialogViewModelFactory = newProjectDialogViewModelFactory;

            _projectService.CurrentProjectChanged += ProjectServiceOnCurrentProjectChanged;

            foreach (var dockableViewViewModelFactory in dockableViewViewModelFactories)
            {
                DockableViewsViewModels.Add(dockableViewViewModelFactory.Create());
            }

            NewProjectCommand = new RelayCommand(NewProject);
            OpenProjectCommand = new RelayCommand(OpenProject);
            _closeProjectCommand = new RelayCommand(CloseProject, CanCloseProject);
            ExitCommand = new RelayCommand(Exit);
        }

        private void NewProject()
        {
            var viewModel = _newProjectDialogViewModelFactory.Create();
            Window.ShowModalChildWindow(viewModel);
        }

        private void OpenProject()
        {
            var projectFilePath = _requestFilePathService.RequestFilePath(ProjectHandlingConstants.ProjectFileName,
                ProjectHandlingConstants.ProjectFileExtensionMask);

            if (projectFilePath != null) _projectService.OpenProject(projectFilePath);
        }

        private void CloseProject()
        {
            _projectService.CloseProject();
        }

        private bool CanCloseProject()
        {
            return _projectService.IsProjectOpen;
        }

        private void Exit()
        {
            Window.Close();
        }

        private void ProjectServiceOnCurrentProjectChanged(object sender, EventArgs eventArgs)
        {
            CurrentProjectName = _projectService.CurrentProject?.Name;
            _closeProjectCommand.RaiseCanExecuteChanged();
        }
    }
}