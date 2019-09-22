using System;
using System.Windows.Input;
using Geisha.Editor.Core;
using Geisha.Editor.ProjectHandling.Model;
using Geisha.Editor.SceneEditor.Model;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.Scene
{
    internal sealed class AddSceneDialogViewModel : ViewModel
    {
        private readonly ICreateEmptySceneService _createEmptySceneService;
        private readonly IProject _project;
        private readonly IProjectFolder _folder;
        private readonly IProperty<string> _sceneName;

        public string SceneName
        {
            get => _sceneName.Get();
            set => _sceneName.Set(value);
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler CloseRequested;

        public AddSceneDialogViewModel(ICreateEmptySceneService createEmptySceneService, IProject project, IProjectFolder folder)
        {
            _createEmptySceneService = createEmptySceneService;
            _project = project;
            _folder = folder;

            var okCommand = new RelayCommand(Ok, CanOk);
            OkCommand = okCommand;
            CancelCommand = new RelayCommand(Cancel);

            _sceneName = CreateProperty<string>(nameof(SceneName));
            _sceneName.Subscribe(_ => okCommand.RaiseCanExecuteChanged());
        }

        private void Ok()
        {
            if (_project != null)
            {
                _createEmptySceneService.CreateEmptyScene(SceneName, _project);
            }
            else
            {
                _createEmptySceneService.CreateEmptyScene(SceneName, _folder);
            }

            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private bool CanOk()
        {
            return !string.IsNullOrWhiteSpace(SceneName);
        }

        private void Cancel()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}