using System.Windows.Controls;
using Geisha.Editor.Core;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleRendererComponent
{
    /// <summary>
    /// Interaction logic for RectangleRendererComponentPropertiesEditorView.xaml
    /// </summary>
    [RegisterViewFor(typeof(RectangleRendererComponentPropertiesEditorViewModel))]
    internal partial class RectangleRendererComponentPropertiesEditorView : UserControl
    {
        public RectangleRendererComponentPropertiesEditorView()
        {
            InitializeComponent();
        }
    }
}