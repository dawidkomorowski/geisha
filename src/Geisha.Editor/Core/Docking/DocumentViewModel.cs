namespace Geisha.Editor.Core.Docking
{
    public sealed class DocumentViewModel : ViewModel
    {
        private readonly IProperty<string> _title;

        public DocumentViewModel(string title, IView view, ViewModel viewModel)
        {
            _title = CreateProperty(nameof(Title), title);

            View = view;
            View.DataContext = viewModel;
        }

        public string Title
        {
            get => _title.Get();
            set => _title.Set(value);
        }

        public IView View { get; }
    }
}