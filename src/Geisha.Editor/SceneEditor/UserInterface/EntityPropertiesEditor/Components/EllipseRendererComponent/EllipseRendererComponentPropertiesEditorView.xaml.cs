using System.Windows.Controls;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.EllipseRendererComponent
{
    /// <summary>
    /// Interaction logic for EllipseRendererComponentPropertiesEditorView.xaml
    /// </summary>
    internal partial class EllipseRendererComponentPropertiesEditorView : UserControl
    {
        public EllipseRendererComponentPropertiesEditorView()
        {
            InitializeComponent();
        }

        public EllipseRendererComponentPropertiesEditorView(EllipseRendererComponentPropertiesEditorViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}