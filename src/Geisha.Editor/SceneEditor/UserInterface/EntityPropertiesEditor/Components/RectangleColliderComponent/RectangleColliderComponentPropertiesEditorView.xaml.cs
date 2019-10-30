using System.Windows.Controls;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleColliderComponent
{
    /// <summary>
    /// Interaction logic for RectangleColliderComponentPropertiesEditorView.xaml
    /// </summary>
    internal partial class RectangleColliderComponentPropertiesEditorView : UserControl
    {
        public RectangleColliderComponentPropertiesEditorView()
        {
            InitializeComponent();
        }

        public RectangleColliderComponentPropertiesEditorView(RectangleColliderComponentPropertiesEditorViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}