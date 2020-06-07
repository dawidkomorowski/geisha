using System.Windows.Controls;
using Geisha.Editor.Core;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.Transform3DComponent
{
    /// <summary>
    /// Interaction logic for Transform3DComponentPropertiesEditorView.xaml
    /// </summary>
    [RegisterViewFor(typeof(Transform3DComponentPropertiesEditorViewModel))]
    internal partial class Transform3DComponentPropertiesEditorView : UserControl
    {
        public Transform3DComponentPropertiesEditorView()
        {
            InitializeComponent();
        }
    }
}