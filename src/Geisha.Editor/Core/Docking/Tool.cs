namespace Geisha.Editor.Core.Docking
{
    public abstract class Tool
    {
        private readonly string _title;
        private readonly IView _view;
        private readonly ViewModel _viewModel;
        private readonly bool _isVisible;

        protected Tool(string title, IView view, ViewModel viewModel, bool isVisible)
        {
            _title = title;
            _view = view;
            _viewModel = viewModel;
            _isVisible = isVisible;
        }

        internal ToolViewModel CreateViewModel()
        {
            return new ToolViewModel(_title, _view, _viewModel, _isVisible);
        }
    }
}