using Geisha.Common.Math;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model.Components;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleColliderComponent
{
    internal sealed class RectangleColliderComponentPropertiesEditorViewModel : ComponentPropertiesEditorViewModel
    {
        private readonly IProperty<Vector2> _dimension;

        public RectangleColliderComponentPropertiesEditorViewModel(RectangleColliderComponentModel componentModel) : base(componentModel)
        {
            _dimension = CreateProperty(nameof(Dimension), componentModel.Dimension);
            _dimension.Subscribe(v => componentModel.Dimension = v);
        }

        public Vector2 Dimension
        {
            get => _dimension.Get();
            set => _dimension.Set(value);
        }
    }
}