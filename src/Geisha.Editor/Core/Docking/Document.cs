namespace Geisha.Editor.Core.Docking
{
    public abstract class Document
    {
        private readonly string _title;
        private readonly IView _view;
        private readonly ViewModel _viewModel;

        protected Document(string title, IView view, ViewModel viewModel)
        {
            _title = title;
            _view = view;
            _viewModel = viewModel;
        }

        internal DocumentViewModel CreateViewModel()
        {
            return new DocumentViewModel(_title, _view, _viewModel);
        }
    }
}