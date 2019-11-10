using System.Windows.Controls;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Rendering;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.EllipseRendererComponent
{
    internal sealed class EllipseRendererComponentPropertiesEditorViewModel : ComponentPropertiesEditorViewModel
    {
        private readonly IProperty<double> _radiusX;
        private readonly IProperty<double> _radiusY;
        private readonly IProperty<Color> _color;
        private readonly IProperty<bool> _fillInterior;
        private readonly IProperty<bool> _visible;
        private readonly IProperty<string> _sortingLayerName;
        private readonly IProperty<int> _orderInLayer;

        public EllipseRendererComponentPropertiesEditorViewModel(EllipseRendererComponentModel componentModel) : base(componentModel)
        {
            _radiusX = CreateProperty(nameof(RadiusX), componentModel.RadiusX);
            _radiusY = CreateProperty(nameof(RadiusY), componentModel.RadiusY);
            _color = CreateProperty(nameof(Color), componentModel.Color);
            _fillInterior = CreateProperty(nameof(FillInterior), componentModel.FillInterior);
            _visible = CreateProperty(nameof(Visible), componentModel.Visible);
            _sortingLayerName = CreateProperty(nameof(SortingLayerName), componentModel.SortingLayerName);
            _orderInLayer = CreateProperty(nameof(OrderInLayer), componentModel.OrderInLayer);

            _radiusX.Subscribe(v => componentModel.RadiusX = v);
            _radiusY.Subscribe(v => componentModel.RadiusY = v);
            _color.Subscribe(v => componentModel.Color = v);
            _fillInterior.Subscribe(v => componentModel.FillInterior = v);
            _visible.Subscribe(v => componentModel.Visible = v);
            _sortingLayerName.Subscribe(v => componentModel.SortingLayerName = v);
            _orderInLayer.Subscribe(v => componentModel.OrderInLayer = v);
        }

        public override Control View { get; }

        public double RadiusX
        {
            get => _radiusX.Get();
            set => _radiusX.Set(value);
        }

        public double RadiusY
        {
            get => _radiusY.Get();
            set => _radiusY.Set(value);
        }

        public Color Color
        {
            get => _color.Get();
            set => _color.Set(value);
        }

        public bool FillInterior
        {
            get => _fillInterior.Get();
            set => _fillInterior.Set(value);
        }

        public bool Visible
        {
            get => _visible.Get();
            set => _visible.Set(value);
        }

        public string SortingLayerName
        {
            get => _sortingLayerName.Get();
            set => _sortingLayerName.Set(value);
        }

        public int OrderInLayer
        {
            get => _orderInLayer.Get();
            set => _orderInLayer.Set(value);
        }
    }
}