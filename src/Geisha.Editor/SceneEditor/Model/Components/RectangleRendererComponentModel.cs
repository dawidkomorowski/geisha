using Geisha.Engine.Core.Math;
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

        public Vector2 Dimensions
        {
            get => _component.Dimensions;
            set => _component.Dimensions = value;
        }

        public Color Color
        {
            get => _component.Color;
            set => _component.Color = value;
        }

        public bool FillInterior
        {
            get => _component.FillInterior;
            set => _component.FillInterior = value;
        }

        public bool Visible
        {
            get => _component.Visible;
            set => _component.Visible = value;
        }

        public string SortingLayerName
        {
            get => _component.SortingLayerName;
            set => _component.SortingLayerName = value;
        }

        public int OrderInLayer
        {
            get => _component.OrderInLayer;
            set => _component.OrderInLayer = value;
        }
    }
}