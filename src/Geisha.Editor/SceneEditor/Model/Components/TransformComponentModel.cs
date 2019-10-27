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

        public double TranslationX
        {
            get => _component.Translation.X;
            set => _component.Translation = _component.Translation.WithX(value);
        }

        public double TranslationY
        {
            get => _component.Translation.Y;
            set => _component.Translation = _component.Translation.WithY(value);
        }

        public double TranslationZ
        {
            get => _component.Translation.Z;
            set => _component.Translation = _component.Translation.WithZ(value);
        }

        public double RotationX
        {
            get => _component.Rotation.X;
            set => _component.Rotation = _component.Rotation.WithX(value);
        }

        public double RotationY
        {
            get => _component.Rotation.Y;
            set => _component.Rotation = _component.Rotation.WithY(value);
        }

        public double RotationZ
        {
            get => _component.Rotation.Z;
            set => _component.Rotation = _component.Rotation.WithZ(value);
        }

        public double ScaleX
        {
            get => _component.Scale.X;
            set => _component.Scale = _component.Scale.WithX(value);
        }

        public double ScaleY
        {
            get => _component.Scale.Y;
            set => _component.Scale = _component.Scale.WithY(value);
        }

        public double ScaleZ
        {
            get => _component.Scale.Z;
            set => _component.Scale = _component.Scale.WithZ(value);
        }
    }
}