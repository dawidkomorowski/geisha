using System.Windows.Input;

namespace Geisha.Editor.Core.ViewModels
{
    public class DockableViewViewModel : ViewModel
    {
        private string _title;
        private bool _isVisible;

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => Set(ref _isVisible, value);
        }

        public ViewModel ViewModel { get; }

        public ICommand ShowCommand { get; }
        public ICommand CloseCommand { get; }

        public DockableViewViewModel(string title, ViewModel viewModel)
        {
            Title = title;
            ViewModel = viewModel;

            ShowCommand = new RelayCommand(() => IsVisible = true);
            CloseCommand = new RelayCommand(() => IsVisible = false);
        }
    }
}