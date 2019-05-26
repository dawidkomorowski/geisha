using Geisha.Editor.Core.ViewModels.Infrastructure;

namespace Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer
{
    public class ProjectExplorerDockableViewViewModelFactory : IDockableViewViewModelFactory
    {
        private readonly ProjectExplorerViewModel _viewModel;

        public ProjectExplorerDockableViewViewModelFactory(ProjectExplorerViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public DockableViewViewModel Create()
        {
            return new DockableViewViewModel("Project Explorer", _viewModel) {IsVisible = true};
        }
    }
}