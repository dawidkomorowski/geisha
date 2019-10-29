using Geisha.Engine.Physics.Components;

namespace Geisha.Editor.SceneEditor.Model.Components
{
    public class CircleColliderComponentModel : IComponentModel
    {
        private readonly CircleColliderComponent _component;

        public CircleColliderComponentModel(CircleColliderComponent component)
        {
            _component = component;
        }

        public string Name => "Circle Collider Component";

        public double Radius
        {
            get => _component.Radius;
            set => _component.Radius = value;
        }
    }
}