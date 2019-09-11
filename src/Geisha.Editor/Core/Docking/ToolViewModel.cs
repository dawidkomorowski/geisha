using System.Windows.Input;
using Geisha.Editor.Core.ViewModels;

namespace Geisha.Editor.Core.Docking
{
    public abstract class ToolViewModel : ViewModel
    {
        private readonly IProperty<bool> _isVisible;
        private readonly IProperty<string> _title;

        protected ToolViewModel(string title, IView view, ViewModel viewModel, bool isVisible)
        {
            _title = CreateProperty(nameof(Title), title);
            _isVisible = CreateProperty(nameof(IsVisible), isVisible);

            View = view;

            View.DataContext = viewModel;

            ShowCommand = new RelayCommand(() => IsVisible = true);
            CloseCommand = new RelayCommand(() => IsVisible = false);
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