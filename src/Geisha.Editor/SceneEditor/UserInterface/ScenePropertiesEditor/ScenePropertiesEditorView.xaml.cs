using System.Windows.Controls;
using Geisha.Editor.Core;

namespace Geisha.Editor.SceneEditor.UserInterface.ScenePropertiesEditor
{
    /// <summary>
    /// Interaction logic for ScenePropertiesEditor.xaml
    /// </summary>
    [RegisterViewFor(typeof(ScenePropertiesEditorViewModel))]
    internal partial class ScenePropertiesEditorView : UserControl
    {
        public ScenePropertiesEditorView()
        {
            InitializeComponent();
        }
    }
}