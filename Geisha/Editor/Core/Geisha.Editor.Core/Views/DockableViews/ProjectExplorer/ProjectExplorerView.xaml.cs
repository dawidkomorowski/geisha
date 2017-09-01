using System.Windows.Controls;
using Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer;
using Geisha.Editor.Core.Views.Infrastructure;

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