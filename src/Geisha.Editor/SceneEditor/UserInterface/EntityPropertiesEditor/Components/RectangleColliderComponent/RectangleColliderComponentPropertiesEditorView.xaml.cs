using System.Windows.Controls;
using Geisha.Editor.Core;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleColliderComponent
{
    /// <summary>
    /// Interaction logic for RectangleColliderComponentPropertiesEditorView.xaml
    /// </summary>
    [RegisterViewFor(typeof(RectangleColliderComponentPropertiesEditorViewModel))]
    internal partial class RectangleColliderComponentPropertiesEditorView : UserControl
    {
        public RectangleColliderComponentPropertiesEditorView()
        {
            InitializeComponent();
        }
    }
}