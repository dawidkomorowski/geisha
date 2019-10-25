using System.Windows.Controls;
using Geisha.Editor.SceneEditor.Model.Components;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TransformComponent
{
    internal sealed class TransformComponentPropertiesEditorViewModel : ComponentPropertiesEditorViewModel
    {
        private readonly TransformComponentModel _componentModel;

        public TransformComponentPropertiesEditorViewModel(TransformComponentModel componentModel) : base(componentModel)
        {
            _componentModel = componentModel;

            View = new TransformComponentPropertiesEditorView(this);
        }

        public override Control View { get; }
    }
}