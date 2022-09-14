using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Components;

namespace Geisha.Editor.SceneEditor.Model.Components
{
    internal sealed class EllipseRendererComponentModel : IComponentModel
    {
        private readonly EllipseRendererComponent _component;

        public EllipseRendererComponentModel(EllipseRendererComponent component)
        {
            _component = component;
        }

        public string Name => "Ellipse Renderer Component";

        public double RadiusX
        {
            get => _component.RadiusX;
            set => _component.RadiusX = value;
        }

        public double RadiusY
        {
            get => _component.RadiusY;
            set => _component.RadiusY = value;
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