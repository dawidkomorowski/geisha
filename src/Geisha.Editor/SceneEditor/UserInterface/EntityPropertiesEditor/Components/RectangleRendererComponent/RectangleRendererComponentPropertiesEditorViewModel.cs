using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Core.Math;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.RectangleRendererComponent
{
    internal sealed class RectangleRendererComponentPropertiesEditorViewModel : ComponentPropertiesEditorViewModel
    {
        private readonly IProperty<Vector2> _dimensions;
        private readonly IProperty<Color> _color;
        private readonly IProperty<bool> _fillInterior;
        private readonly IProperty<bool> _visible;
        private readonly IProperty<string> _sortingLayerName;
        private readonly IProperty<int> _orderInLayer;

        public RectangleRendererComponentPropertiesEditorViewModel(RectangleRendererComponentModel componentModel) : base(componentModel)
        {
            _dimensions = CreateProperty(nameof(Dimensions), componentModel.Dimensions);
            _color = CreateProperty(nameof(Color), componentModel.Color);
            _fillInterior = CreateProperty(nameof(FillInterior), componentModel.FillInterior);
            _visible = CreateProperty(nameof(Visible), componentModel.Visible);
            _sortingLayerName = CreateProperty(nameof(SortingLayerName), componentModel.SortingLayerName);
            _orderInLayer = CreateProperty(nameof(OrderInLayer), componentModel.OrderInLayer);

            _dimensions.Subscribe(v => componentModel.Dimensions = v);
            _color.Subscribe(v => componentModel.Color = v);
            _fillInterior.Subscribe(v => componentModel.FillInterior = v);
            _visible.Subscribe(v => componentModel.Visible = v);
            _sortingLayerName.Subscribe(v => componentModel.SortingLayerName = v);
            _orderInLayer.Subscribe(v => componentModel.OrderInLayer = v);
        }

        public Vector2 Dimensions
        {
            get => _dimensions.Get();
            set => _dimensions.Set(value);
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