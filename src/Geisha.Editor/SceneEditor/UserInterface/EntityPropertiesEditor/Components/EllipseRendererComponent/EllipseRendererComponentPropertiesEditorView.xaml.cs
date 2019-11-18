using System.Windows.Controls;
using Geisha.Editor.Core;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.EllipseRendererComponent
{
    /// <summary>
    /// Interaction logic for EllipseRendererComponentPropertiesEditorView.xaml
    /// </summary>
    [RegisterViewFor(typeof(EllipseRendererComponentPropertiesEditorViewModel))]
    internal partial class EllipseRendererComponentPropertiesEditorView : UserControl
    {
        public EllipseRendererComponentPropertiesEditorView()
        {
            InitializeComponent();
        }
    }
}