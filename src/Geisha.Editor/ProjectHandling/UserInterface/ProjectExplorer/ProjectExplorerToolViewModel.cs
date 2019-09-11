using Geisha.Editor.Core.Docking;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer
{
    public sealed class ProjectExplorerToolViewModel : ToolViewModel
    {
        public ProjectExplorerToolViewModel(ProjectExplorerViewModel viewModel) : base("Project Explorer", new ProjectExplorerView(), viewModel, true)
        {
        }
    }
}