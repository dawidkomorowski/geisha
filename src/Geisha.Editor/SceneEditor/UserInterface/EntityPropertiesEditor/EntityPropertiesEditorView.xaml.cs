using System.Windows.Controls;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor
{
    /// <summary>
    /// Interaction logic for EntityPropertiesEditorView.xaml
    /// </summary>
    internal partial class EntityPropertiesEditorView : UserControl
    {
        public EntityPropertiesEditorView()
        {
            InitializeComponent();
        }

        public EntityPropertiesEditorView(EntityPropertiesEditorViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}