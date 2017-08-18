using System.Windows.Controls;
using Geisha.Editor.Core.ViewModels.DockableViews.ProjectExplorer;
using Geisha.Editor.Core.Views.Infrastructure;

namespace Geisha.Editor.Core.Views.DockableViews.ProjectExplorer
{
    /// <summary>
    /// Interaction logic for ProjectExplorerView.xaml
    /// </summary>
    [ViewModel(typeof(ProjectExplorerViewModel))]
    public partial class ProjectExplorerView : UserControl
    {
        public ProjectExplorerView()
        {
            InitializeComponent();
        }
    }
}