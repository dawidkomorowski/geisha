using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Geisha.Editor.Core.Infrastructure;
using Geisha.Editor.Core.ViewModels.Infrastructure;
using Geisha.Editor.ProjectHandling.Domain;
using Geisha.Editor.ProjectHandling.Infrastructure;
using Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog;

namespace Geisha.Editor
{
    public class MainViewModel : ViewModel, IWindowContext
    {
        private readonly RelayCommand _closeProjectCommand;
        private readonly INewProjectDialogViewModelFactory _newProjectDialogViewModelFactory;
        private readonly IProjectServiceObsolete _projectService;
        private readonly IRequestFilePathService _requestFilePathService;
        private readonly IVersionProvider _versionProvider;

        private string _currentProjectName;

        public MainViewModel(IVersionProvider versionProvider, IRequestFilePathService requestFilePathService, IProjectServiceObsolete projectService,
            INewProjectDialogViewModelFactory newProjectDialogViewModelFactory, IEnumerable<IDockableViewViewModelFactory> dockableViewViewModelFactories)
        {
            _versionProvider = versionProvider;
            _requestFilePathService = requestFilePathService;
            _projectService = projectService;
            _newProjectDialogViewModelFactory = newProjectDialogViewModelFactory;

            _projectService.CurrentProjectChanged += ProjectServiceOnCurrentProjectChanged;

            foreach (var dockableViewViewModelFactory in dockableViewViewModelFactories)
                DockableViewsViewModels.Add(dockableViewViewModelFactory.Create());

            NewProjectCommand = new RelayCommand(NewProject);
            OpenProjectCommand = new RelayCommand(OpenProject);
            _closeProjectCommand = new RelayCommand(CloseProject, CanCloseProject);
            ExitCommand = new RelayCommand(Exit);
        }

        private string ApplicationVersion => _versionProvider.GetCurrentVersion().ToString();

        private string CurrentProjectName
        {
            get => _currentProjectName;
            set => Set(ref _currentProjectName, value);
        }

        public ObservableCollection<DockableViewViewModel> DockableViewsViewModels { get; set; } = new ObservableCollection<DockableViewViewModel>();

        [DependsOnProperty(nameof(CurrentProjectName))]
        public string ApplicationTitle
        {
            get
            {
                var prefix = string.IsNullOrEmpty(CurrentProjectName) ? string.Empty : $"{CurrentProjectName} - ";
                return $"{prefix}Geisha Editor {ApplicationVersion}";
            }
        }

        public ICommand NewProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand CloseProjectCommand => _closeProjectCommand;
        public ICommand ExitCommand { get; }

        public IWindow Window { get; set; }

        private void NewProject()
        {
            var viewModel = _newProjectDialogViewModelFactory.Create();
            Window.ShowModalChildWindow(viewModel);
        }

        private void OpenProject()
        {
            var projectFilePath = _requestFilePathService.RequestFilePath(ProjectHandlingConstants.ProjectFileTypeDisplayName,
                ProjectHandlingConstants.ProjectFileExtensionFilter);

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