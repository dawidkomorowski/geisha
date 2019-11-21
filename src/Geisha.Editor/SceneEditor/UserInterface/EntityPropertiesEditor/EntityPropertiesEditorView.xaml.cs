using System.Windows.Controls;
using Geisha.Editor.Core;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor
{
    /// <summary>
    /// Interaction logic for EntityPropertiesEditorView.xaml
    /// </summary>
    [RegisterViewFor(typeof(EntityPropertiesEditorViewModel))]
    internal partial class EntityPropertiesEditorView : UserControl
    {
        public EntityPropertiesEditorView()
        {
            InitializeComponent();
        }
    }
}