using System.ComponentModel.Composition;
using Geisha.Editor.Core.ViewModels.Infrastructure;

namespace Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer
{
    [Export(typeof(IDockableViewViewModelFactory))]
    public class ProjectExplorerDockableViewViewModelFactory : IDockableViewViewModelFactory
    {
        private readonly ProjectExplorerViewModel _viewModel;

        [ImportingConstructor]
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