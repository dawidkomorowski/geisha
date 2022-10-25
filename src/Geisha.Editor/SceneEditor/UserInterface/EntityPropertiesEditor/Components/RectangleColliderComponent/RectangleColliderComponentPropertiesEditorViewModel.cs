using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Core.Math;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleColliderComponent
{
    internal sealed class RectangleColliderComponentPropertiesEditorViewModel : ComponentPropertiesEditorViewModel
    {
        private readonly IProperty<Vector2> _dimensions;

        public RectangleColliderComponentPropertiesEditorViewModel(RectangleColliderComponentModel componentModel) : base(componentModel)
        {
            _dimensions = CreateProperty(nameof(Dimensions), componentModel.Dimensions);
            _dimensions.Subscribe(v => componentModel.Dimensions = v);
        }

        public Vector2 Dimensions
        {
            get => _dimensions.Get();
            set => _dimensions.Set(value);
        }
    }
}