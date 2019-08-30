using System.Windows.Controls;
using Geisha.Editor.Core.Views.Infrastructure;
using Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer;

namespace Geisha.Editor.Core.Views.DockableViews.ProjectExplorer
{
    [ViewModel(typeof(ProjectExplorerViewModel))]
    public partial class ProjectExplorerView : UserControl
    {
        public ProjectExplorerView()
        {
            InitializeComponent();
        }
    }
}