using System.Windows.Controls;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleRendererComponent
{
    /// <summary>
    /// Interaction logic for RectangleRendererComponentPropertiesEditorView.xaml
    /// </summary>
    internal partial class RectangleRendererComponentPropertiesEditorView : UserControl
    {
        public RectangleRendererComponentPropertiesEditorView()
        {
            InitializeComponent();
        }

        public RectangleRendererComponentPropertiesEditorView(RectangleRendererComponentPropertiesEditorViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}