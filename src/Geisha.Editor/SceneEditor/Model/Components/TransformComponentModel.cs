using Geisha.Common.Math;
using Geisha.Engine.Core.Components;

namespace Geisha.Editor.SceneEditor.Model.Components
{
    public sealed class TransformComponentModel : IComponentModel
    {
        private readonly TransformComponent _component;

        public TransformComponentModel(TransformComponent component)
        {
            _component = component;
        }

        public string Name => "Transform Component";

        public Vector3 Translation
        {
            get => _component.Translation;
            set => _component.Translation = value;
        }

        public Vector3 Rotation
        {
            get => _component.Rotation;
            set => _component.Rotation = value;
        }

        public Vector3 Scale
        {
            get => _component.Scale;
            set => _component.Scale = value;
        }
    }
}