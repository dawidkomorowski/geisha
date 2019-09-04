using System.Windows.Input;

namespace Geisha.Editor.Core.ViewModels
{
    public class DockableViewViewModel : ViewModel
    {
        private readonly IProperty<string> _title;
        private readonly IProperty<bool> _isVisible;

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

        public ViewModel ViewModel { get; }

        public ICommand ShowCommand { get; }
        public ICommand CloseCommand { get; }

        public DockableViewViewModel(string title, ViewModel viewModel)
        {
            _title = CreateProperty(nameof(Title), title);
            _isVisible = CreateProperty(nameof(IsVisible), false);

            ViewModel = viewModel;

            ShowCommand = new RelayCommand(() => IsVisible = true);
            CloseCommand = new RelayCommand(() => IsVisible = false);
        }
    }
}