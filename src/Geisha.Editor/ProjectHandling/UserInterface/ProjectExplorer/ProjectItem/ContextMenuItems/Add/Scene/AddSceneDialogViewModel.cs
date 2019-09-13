using System;
using System.Windows.Input;
using Geisha.Editor.Core;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer.ProjectItem.ContextMenuItems.Add.Scene
{
    internal sealed class AddSceneDialogViewModel : ViewModel
    {
        private readonly IProperty<string> _sceneName;

        public string SceneName
        {
            get => _sceneName.Get();
            set => _sceneName.Set(value);
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public event EventHandler CloseRequested;

        public AddSceneDialogViewModel()
        {
            var okCommand = new RelayCommand(Ok, CanOk);
            OkCommand = okCommand;
            CancelCommand = new RelayCommand(Cancel);

            _sceneName = CreateProperty<string>(nameof(SceneName));
            _sceneName.Subscribe(_ => okCommand.RaiseCanExecuteChanged());
        }

        private void Ok()
        {
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