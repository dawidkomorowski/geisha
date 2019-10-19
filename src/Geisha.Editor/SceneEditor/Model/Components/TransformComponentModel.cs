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
    }
}