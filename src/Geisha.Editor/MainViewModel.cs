using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Geisha.Editor.Core;
using Geisha.Editor.Core.Docking;
using Geisha.Editor.Core.ViewModels;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog;

namespace Geisha.Editor
{
    public class MainViewModel : ViewModel, IWindowContext
    {
        private readonly IProperty<string> _currentProjectName;
        private readonly IComputedProperty<string> _applicationTitle;
        private readonly RelayCommand _closeProjectCommand;
        private readonly INewProjectDialogViewModelFactory _newProjectDialogViewModelFactory;
        private readonly IProjectService _projectService;
        private readonly IOpenFileDialogService _openFileDialogService;
        private readonly IVersionProvider _versionProvider;

        public MainViewModel(IVersionProvider versionProvider, IOpenFileDialogService openFileDialogService, IProjectService projectService,
            INewProjectDialogViewModelFactory newProjectDialogViewModelFactory, IEnumerable<Tool> tools)
        {
            _versionProvider = versionProvider;
            _openFileDialogService = openFileDialogService;
            _projectService = projectService;
            _newProjectDialogViewModelFactory = newProjectDialogViewModelFactory;

            _projectService.CurrentProjectChanged += ProjectServiceOnCurrentProjectChanged;

            foreach (var tool in tools)
            {
                ToolsViewModels.Add(tool.CreateViewModel());
            }

            _currentProjectName = CreateProperty<string>(nameof(CurrentProjectName));
            _applicationTitle = CreateComputedProperty(nameof(ApplicationTitle), _currentProjectName, currentProjectName =>
            {
                var prefix = string.IsNullOrEmpty(currentProjectName) ? string.Empty : $"{CurrentProjectName} - ";
                return $"{prefix}Geisha Editor {ApplicationVersion}";
            });

            NewProjectCommand = new RelayCommand(NewProject);
            OpenProjectCommand = new RelayCommand(OpenProject);
            _closeProjectCommand = new RelayCommand(CloseProject, CanCloseProject);
            ExitCommand = new RelayCommand(Exit);
        }

        private string ApplicationVersion => _versionProvider.GetCurrentVersion().ToString();

        private string CurrentProjectName
        {
            get => _currentProjectName.Get();
            set => _currentProjectName.Set(value);
        }

        public ObservableCollection<ToolViewModel> ToolsViewModels { get; set; } = new ObservableCollection<ToolViewModel>();

        public string ApplicationTitle => _applicationTitle.Get();

        public ICommand NewProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand CloseProjectCommand => _closeProjectCommand;
        public ICommand ExitCommand { get; }

        public IWindow Window { get; set; }

        public sealed class NewProjectDialogRequestedEventArgs : EventArgs
        {
            public NewProjectDialogRequestedEventArgs(NewProjectDialogViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public NewProjectDialogViewModel ViewModel { get; }
        }

        public event EventHandler<NewProjectDialogRequestedEventArgs> NewProjectDialogRequested;

        private void NewProject()
        {
            var viewModel = _newProjectDialogViewModelFactory.Create();
            NewProjectDialogRequested?.Invoke(this, new NewProjectDialogRequestedEventArgs(viewModel));
        }

        private void OpenProject()
        {
            var projectFilePath = _openFileDialogService.AskForFilePath(ProjectHandlingConstants.ProjectFileTypeDisplayName,
                ProjectHandlingConstants.ProjectFileExtensionFilter);

            if (projectFilePath != null) _projectService.OpenProject(projectFilePath);
        }

        private void CloseProject()
        {
            _projectService.CloseProject();
        }

        private bool CanCloseProject()
        {
            return _projectService.ProjectIsOpen;
        }

        private void Exit()
        {
            Window.Close();
        }

        private void ProjectServiceOnCurrentProjectChanged(object sender, EventArgs eventArgs)
        {
            CurrentProjectName = _projectService.ProjectIsOpen ? _projectService.CurrentProject.Name : string.Empty;
            _closeProjectCommand.RaiseCanExecuteChanged();
        }
    }
}