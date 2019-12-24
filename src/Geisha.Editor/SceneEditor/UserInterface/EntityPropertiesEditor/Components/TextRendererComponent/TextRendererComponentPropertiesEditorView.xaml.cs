using System.Windows.Controls;
using Geisha.Editor.Core;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TextRendererComponent
{
    /// <summary>
    /// Interaction logic for TextRendererComponentPropertiesEditorView.xaml
    /// </summary>
    [RegisterViewFor(typeof(TextRendererComponentPropertiesEditorViewModel))]
    internal sealed partial class TextRendererComponentPropertiesEditorView : UserControl
    {
        public TextRendererComponentPropertiesEditorView()
        {
            InitializeComponent();
        }
    }
}