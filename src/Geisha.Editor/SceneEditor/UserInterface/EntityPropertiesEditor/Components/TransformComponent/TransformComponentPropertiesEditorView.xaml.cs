using System.Windows.Controls;
using Geisha.Editor.Core;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TransformComponent
{
    /// <summary>
    /// Interaction logic for TransformComponentPropertiesEditorView.xaml
    /// </summary>
    [RegisterViewFor(typeof(TransformComponentPropertiesEditorViewModel))]
    internal partial class TransformComponentPropertiesEditorView : UserControl
    {
        public TransformComponentPropertiesEditorView()
        {
            InitializeComponent();
        }
    }
}