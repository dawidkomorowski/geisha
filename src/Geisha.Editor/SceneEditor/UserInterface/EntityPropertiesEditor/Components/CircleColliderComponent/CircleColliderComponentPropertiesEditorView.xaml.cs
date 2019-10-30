using System.Windows.Controls;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.CircleColliderComponent
{
    /// <summary>
    /// Interaction logic for CircleColliderComponentPropertiesEditorView.xaml
    /// </summary>
    internal partial class CircleColliderComponentPropertiesEditorView : UserControl
    {
        public CircleColliderComponentPropertiesEditorView()
        {
            InitializeComponent();
        }

        public CircleColliderComponentPropertiesEditorView(CircleColliderComponentPropertiesEditorViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}