using Geisha.Common.Math;
using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Rendering;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TextRendererComponent
{
    internal sealed class TextRendererComponentPropertiesEditorViewModel : ComponentPropertiesEditorViewModel
    {
        private readonly IProperty<string> _text;
        private readonly IProperty<FontSize> _fontSize;
        private readonly IProperty<Color> _color;
        private readonly IProperty<bool> _visible;
        private readonly IProperty<string> _sortingLayerName;
        private readonly IProperty<int> _orderInLayer;

        public TextRendererComponentPropertiesEditorViewModel(TextRendererComponentModel componentModel) : base(componentModel)
        {
            _text = CreateProperty(nameof(Text), componentModel.Text);
            _fontSize = CreateProperty(nameof(FontSize), componentModel.FontSize);
            _color = CreateProperty(nameof(Color), componentModel.Color);
            _visible = CreateProperty(nameof(Visible), componentModel.Visible);
            _sortingLayerName = CreateProperty(nameof(SortingLayerName), componentModel.SortingLayerName);
            _orderInLayer = CreateProperty(nameof(OrderInLayer), componentModel.OrderInLayer);

            _text.Subscribe(v => componentModel.Text = v);
            _fontSize.Subscribe(v => componentModel.FontSize = v);
            _color.Subscribe(v => componentModel.Color = v);
            _visible.Subscribe(v => componentModel.Visible = v);
            _sortingLayerName.Subscribe(v => componentModel.SortingLayerName = v);
            _orderInLayer.Subscribe(v => componentModel.OrderInLayer = v);
        }

        public string Text
        {
            get => _text.Get();
            set => _text.Set(value);
        }

        public FontSize FontSize
        {
            get => _fontSize.Get();
            set => _fontSize.Set(value);
        }

        public Color Color
        {
            get => _color.Get();
            set => _color.Set(value);
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