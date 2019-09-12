using Geisha.Editor.Core.Docking;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer
{
    public sealed class ProjectExplorerTool : Tool
    {
        public ProjectExplorerTool(ProjectExplorerViewModel viewModel) : base("Project Explorer", new ProjectExplorerView(), viewModel, true)
        {
        }
    }
}