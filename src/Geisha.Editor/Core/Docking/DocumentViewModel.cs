namespace Geisha.Editor.Core.Docking
{
    public sealed class DocumentViewModel : ViewModel
    {
        private readonly IProperty<string> _title;
        private readonly IProperty<bool> _isSelected;

        public DocumentViewModel(string title, IView view, ViewModel viewModel)
        {
            _title = CreateProperty(nameof(Title), title);
            _isSelected = CreateProperty(nameof(IsSelected), false);

            View = view;
            View.DataContext = viewModel;

            if (viewModel is IDocumentSelectedSubscriber documentSelectedSubscriber)
            {
                _isSelected.Subscribe(value =>
                {
                    if (value) documentSelectedSubscriber.OnDocumentSelected();
                });
            }

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
    }
}