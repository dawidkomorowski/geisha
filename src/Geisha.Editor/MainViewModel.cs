using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Geisha.Editor.Core;
using Geisha.Editor.Core.Docking;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.ProjectHandling.UserInterface.NewProjectDialog;

namespace Geisha.Editor
{
    public class MainViewModel : ViewModel
    {
        private readonly IProperty<string> _currentProjectName;
        private readonly IComputedProperty<string> _applicationTitle;
        private readonly RelayCommand _closeProjectCommand;
        private readonly INewProjectDialogViewModelFactory _newProjectDialogViewModelFactory;
        private readonly IProjectService _projectService;
        private readonly IVersionProvider _versionProvider;

        public MainViewModel(IVersionProvider versionProvider, IProjectService projectService,
            INewProjectDialogViewModelFactory newProjectDialogViewModelFactory, IEnumerable<Tool> tools, Document document)
        {
            _versionProvider = versionProvider;
            _projectService = projectService;
            _newProjectDialogViewModelFactory = newProjectDialogViewModelFactory;

            _projectService.CurrentProjectChanged += ProjectServiceOnCurrentProjectChanged;

            foreach (var tool in tools)
            {
                ToolsViewModels.Add(tool.CreateViewModel());
            }

            DocumentsViewModels.Add(document.CreateViewModel());

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

        public ObservableCollection<ToolViewModel> ToolsViewModels { get; } = new ObservableCollection<ToolViewModel>();
        public ObservableCollection<DocumentViewModel> DocumentsViewModels { get; } = new ObservableCollection<DocumentViewModel>();

        public string ApplicationTitle => _applicationTitle.Get();

        public ICommand NewProjectCommand { get; }
        public ICommand OpenProjectCommand { get; }
        public ICommand CloseProjectCommand => _closeProjectCommand;
        public ICommand ExitCommand { get; }

        public sealed class NewProjectDialogRequestedEventArgs : EventArgs
        {
            public NewProjectDialogRequestedEventArgs(NewProjectDialogViewModel viewModel)
            {
                ViewModel = viewModel;
            }

            public NewProjectDialogViewModel ViewModel { get; }
        }

        public event EventHandler<NewProjectDialogRequestedEventArgs> NewProjectDialogRequested;
        public event EventHandler<OpenFileDialogEventArgs> OpenFileDialogRequested;
        public event EventHandler CloseRequested;

        private void NewProject()
        {
            var viewModel = _newProjectDialogViewModelFactory.Create();
            NewProjectDialogRequested?.Invoke(this, new NewProjectDialogRequestedEventArgs(viewModel));
        }

        private void OpenProject()
        {
            OpenFileDialogRequested?.Invoke(this, OpenFileDialogEventArgs.AskForFilePath(
                    ProjectHandlingConstants.ProjectFileTypeDisplayName,
                    ProjectHandlingConstants.ProjectFileExtensionFilter)
                .AndContinueWith(projectFilePath =>
                {
                    if (projectFilePath != null) _projectService.OpenProject(projectFilePath);
                }));
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
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ProjectServiceOnCurrentProjectChanged(object sender, EventArgs eventArgs)
        {
            CurrentProjectName = _projectService.ProjectIsOpen ? _projectService.CurrentProject.Name : string.Empty;
            _closeProjectCommand.RaiseCanExecuteChanged();
        }
    }
}