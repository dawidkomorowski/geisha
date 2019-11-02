using Geisha.Engine.Rendering.Components;

namespace Geisha.Editor.SceneEditor.Model.Components
{
    internal sealed class RectangleRendererComponentModel : IComponentModel
    {
        private readonly RectangleRendererComponent _component;

        public RectangleRendererComponentModel(RectangleRendererComponent component)
        {
            _component = component;
        }

        public string Name => "Rectangle Renderer Component";
    }
}