using Geisha.Editor.Core;
using Geisha.Editor.SceneEditor.Model.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;

namespace Geisha.Editor.SceneEditor.UserInterface.EntityPropertiesEditor.Components.TextRendererComponent
{
    internal sealed class TextRendererComponentPropertiesEditorViewModel : ComponentPropertiesEditorViewModel
    {
        private readonly IProperty<string> _text;
        private readonly IProperty<string> _fontFamilyName;
        private readonly IProperty<FontSize> _fontSize;
        private readonly IProperty<Color> _color;
        private readonly IProperty<double> _maxWidth;
        private readonly IProperty<double> _maxHeight;
        private readonly IProperty<TextAlignment> _textAlignment;
        private readonly IProperty<ParagraphAlignment> _paragraphAlignment;
        private readonly IProperty<Vector2> _pivot;
        private readonly IProperty<bool> _clipToLayoutBox;
        private readonly IProperty<bool> _visible;
        private readonly IProperty<string> _sortingLayerName;
        private readonly IProperty<int> _orderInLayer;

        public TextRendererComponentPropertiesEditorViewModel(TextRendererComponentModel componentModel) : base(componentModel)
        {
            _text = CreateProperty(nameof(Text), componentModel.Text);
            _fontFamilyName = CreateProperty(nameof(FontFamilyName), componentModel.FontFamilyName);
            _fontSize = CreateProperty(nameof(FontSize), componentModel.FontSize);
            _color = CreateProperty(nameof(Color), componentModel.Color);
            _maxWidth = CreateProperty(nameof(MaxWidth), componentModel.MaxWidth);
            _maxHeight = CreateProperty(nameof(MaxHeight), componentModel.MaxHeight);
            _textAlignment = CreateProperty(nameof(TextAlignment), componentModel.TextAlignment);
            _paragraphAlignment = CreateProperty(nameof(ParagraphAlignment), componentModel.ParagraphAlignment);
            _pivot = CreateProperty(nameof(Pivot), componentModel.Pivot);
            _clipToLayoutBox = CreateProperty(nameof(ClipToLayoutBox), componentModel.ClipToLayoutBox);
            _visible = CreateProperty(nameof(Visible), componentModel.Visible);
            _sortingLayerName = CreateProperty(nameof(SortingLayerName), componentModel.SortingLayerName);
            _orderInLayer = CreateProperty(nameof(OrderInLayer), componentModel.OrderInLayer);

            _text.Subscribe(v => componentModel.Text = v);
            _fontFamilyName.Subscribe(v => componentModel.FontFamilyName = v);
            _fontSize.Subscribe(v => componentModel.FontSize = v);
            _color.Subscribe(v => componentModel.Color = v);
            _maxWidth.Subscribe(v => componentModel.MaxWidth = v);
            _maxHeight.Subscribe(v => componentModel.MaxHeight = v);
            _textAlignment.Subscribe(v => componentModel.TextAlignment = v);
            _paragraphAlignment.Subscribe(v => componentModel.ParagraphAlignment = v);
            _pivot.Subscribe(v => componentModel.Pivot = v);
            _clipToLayoutBox.Subscribe(v => componentModel.ClipToLayoutBox = v);
            _visible.Subscribe(v => componentModel.Visible = v);
            _sortingLayerName.Subscribe(v => componentModel.SortingLayerName = v);
            _orderInLayer.Subscribe(v => componentModel.OrderInLayer = v);
        }

        public string Text
        {
            get => _text.Get();
            set => _text.Set(value);
        }

        public string FontFamilyName
        {
            get => _fontFamilyName.Get();
            set => _fontFamilyName.Set(value);
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

        public double MaxWidth
        {
            get => _maxWidth.Get();
            set => _maxWidth.Set(value);
        }

        public double MaxHeight
        {
            get => _maxHeight.Get();
            set => _maxHeight.Set(value);
        }

        public TextAlignment TextAlignment
        {
            get => _textAlignment.Get();
            set => _textAlignment.Set(value);
        }

        public ParagraphAlignment ParagraphAlignment
        {
            get => _paragraphAlignment.Get();
            set => _paragraphAlignment.Set(value);
        }

        public Vector2 Pivot
        {
            get => _pivot.Get();
            set => _pivot.Set(value);
        }

        public bool ClipToLayoutBox
        {
            get => _clipToLayoutBox.Get();
            set => _clipToLayoutBox.Set(value);
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