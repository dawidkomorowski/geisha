using System.Windows.Controls;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TransformComponent
{
    /// <summary>
    /// Interaction logic for TransformComponentPropertiesEditorView.xaml
    /// </summary>
    internal partial class TransformComponentPropertiesEditorView : UserControl
    {
        public TransformComponentPropertiesEditorView()
        {
            InitializeComponent();
        }

        public TransformComponentPropertiesEditorView(TransformComponentPropertiesEditorViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}