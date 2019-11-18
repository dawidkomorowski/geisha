using System.Windows.Controls;
using Geisha.Editor.Core;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.CircleColliderComponent
{
    /// <summary>
    /// Interaction logic for CircleColliderComponentPropertiesEditorView.xaml
    /// </summary>
    [RegisterViewFor(typeof(CircleColliderComponentPropertiesEditorViewModel))]
    internal partial class CircleColliderComponentPropertiesEditorView : UserControl
    {
        public CircleColliderComponentPropertiesEditorView()
        {
            InitializeComponent();
        }
    }
}