namespace Geisha.Editor.Core.Docking
{
    public sealed class DocumentViewModel : ViewModel
    {
        private readonly IProperty<string> _title;
        private readonly IProperty<bool> _isSelected;
        private readonly DocumentContentViewModel _viewModel;

        public DocumentViewModel(string title, IView view, DocumentContentViewModel viewModel)
        {
            _title = CreateProperty(nameof(Title), title);
            _isSelected = CreateProperty(nameof(IsSelected), false);

            View = view;
            View.DataContext = viewModel;
            _viewModel = viewModel;

            _isSelected.Subscribe(value =>
            {
                if (value) viewModel.OnDocumentSelected();
            });

            IsSelected = true;
        }

        public string Title
        {
            get => _title.Get();
            set => _title.Set(value);
        }

        public bool IsSelected
        {
            get => _isSelected.Get();
            set => _isSelected.Set(value);
        }

        public IView View { get; }

        public void Save()
        {
            _viewModel.SaveDocument();
        }
    }
}