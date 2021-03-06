﻿using System.Windows.Input;

namespace Geisha.Editor.Core.Docking
{
    public sealed class ToolViewModel : ViewModel
    {
        private readonly IProperty<bool> _isVisible;
        private readonly IProperty<string> _title;

        public ToolViewModel(string title, IView view, ViewModel viewModel, bool isVisible)
        {
            _title = CreateProperty(nameof(Title), title);
            _isVisible = CreateProperty(nameof(IsVisible), isVisible);

            View = view;
            View.DataContext = viewModel;

            ShowCommand = RelayCommand.Create(() => IsVisible = true);
            CloseCommand = RelayCommand.Create(() => IsVisible = false);
        }

        public string Title
        {
            get => _title.Get();
            set => _title.Set(value);
        }

        public bool IsVisible
        {
            get => _isVisible.Get();
            set => _isVisible.Set(value);
        }

        public ICommand ShowCommand { get; }
        public ICommand CloseCommand { get; }

        public IView View { get; }
    }
}