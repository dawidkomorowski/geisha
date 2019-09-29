namespace Geisha.Editor.Core.Docking
{
    internal sealed class Document
    {
        private readonly string _title;
        private readonly IView _view;
        private readonly ViewModel _viewModel;

        public Document(string title, IView view, ViewModel viewModel)
        {
            _title = title;
            _view = view;
            _viewModel = viewModel;
        }

        public DocumentViewModel CreateViewModel()
        {
            return new DocumentViewModel(_title, _view, _viewModel);
        }
    }
}