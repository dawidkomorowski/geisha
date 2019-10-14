using System.Windows.Controls;

namespace Geisha.Editor.SceneEditor.UserInterface.ScenePropertiesEditor
{
    /// <summary>
    /// Interaction logic for ScenePropertiesEditor.xaml
    /// </summary>
    internal partial class ScenePropertiesEditorView : UserControl
    {
        public ScenePropertiesEditorView()
        {
            InitializeComponent();
        }

        public ScenePropertiesEditorView(ScenePropertiesEditorViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}