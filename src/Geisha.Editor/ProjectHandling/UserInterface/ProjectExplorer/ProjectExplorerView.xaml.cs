using System.Windows.Controls;
using Geisha.Editor.Core.Views;

namespace Geisha.Editor.ProjectHandling.UserInterface.ProjectExplorer
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