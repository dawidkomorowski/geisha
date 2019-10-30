using Geisha.Engine.Physics.Components;

namespace Geisha.Editor.SceneEditor.Model.Components
{
    public class RectangleColliderComponentModel : IComponentModel
    {
        private readonly RectangleColliderComponent _component;

        public RectangleColliderComponentModel(RectangleColliderComponent component)
        {
            _component = component;
        }

        public string Name => "Rectangle Collider Component";

        public double DimensionX
        {
            get => _component.Dimension.X;
            set => _component.Dimension = _component.Dimension.WithX(value);
        }

        public double DimensionY
        {
            get => _component.Dimension.Y;
            set => _component.Dimension = _component.Dimension.WithY(value);
        }
    }
}