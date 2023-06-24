using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Rendering.Systems;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Text renderer component enables entity with text rendering functionality.
    /// </summary>
    /// <remarks>
    ///     <see cref="TextRendererComponent" /> allows to specify formatting and layout properties of how the text content
    ///     should be presented. Text is automatically wrapped to fit in the layout box.
    /// </remarks>
    [ComponentId("Geisha.Engine.Rendering.TextRendererComponent")]
    public sealed class TextRendererComponent : Renderer2DComponent
    {
        private TextNode? _textNode;
        private string _text = string.Empty;
        private string _fontFamilyName = "Consolas";
        private FontSize _fontSize = FontSize.FromDips(20);
        private Color _color = Color.Black;
        private double _maxWidth = 500;
        private double _maxHeight = 500;
        private TextAlignment _textAlignment = TextAlignment.Leading;
        private ParagraphAlignment _paragraphAlignment = ParagraphAlignment.Near;
        private Vector2 _pivot;
        private bool _clipToLayoutBox;

        internal TextRendererComponent(Entity entity) : base(entity)
        {
        }

        internal TextNode? TextNode
        {
            get => _textNode;
            set
            {
                if (_textNode is null && value is not null)
                {
                    value.Text = _text;
                    value.FontFamilyName = _fontFamilyName;
                    value.FontSize = _fontSize;
                    value.Color = _color;
                    value.MaxWidth = _maxWidth;
                    value.MaxHeight = _maxHeight;
                    value.TextAlignment = _textAlignment;
                    value.ParagraphAlignment = _paragraphAlignment;
                    value.Pivot = _pivot;
                    value.ClipToLayoutBox = _clipToLayoutBox;
                }

                if (_textNode is not null && value is null)
                {
                    _text = _textNode.Text;
                    _fontFamilyName = _textNode.FontFamilyName;
                    _fontSize = _textNode.FontSize;
                    _color = _textNode.Color;
                    _maxWidth = _textNode.MaxWidth;
                    _maxHeight = _textNode.MaxHeight;
                    _textAlignment = _textNode.TextAlignment;
                    _paragraphAlignment = _textNode.ParagraphAlignment;
                    _pivot = _textNode.Pivot;
                    _clipToLayoutBox = _textNode.ClipToLayoutBox;
                }

                _textNode = value;
            }
        }


        /// <summary>
        ///     Gets or sets text content to be rendered.
        /// </summary>
        public string Text
        {
            get => TextNode is null ? _text : TextNode.Text;
            set
            {
                if (TextNode is null)
                {
                    _text = value;
                }
                else
                {
                    TextNode.Text = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets font family name used for text rendering. Default is Consolas.
        /// </summary>
        /// <remarks>This property allows to use fonts installed in operating system by specifying font family name.</remarks>
        public string FontFamilyName
        {
            get => TextNode is null ? _fontFamilyName : TextNode.FontFamilyName;
            set
            {
                if (TextNode is null)
                {
                    _fontFamilyName = value;
                }
                else
                {
                    TextNode.FontFamilyName = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets size of font used for text rendering. Default size is 20 pixels.
        /// </summary>
        public FontSize FontSize
        {
            get => TextNode?.FontSize ?? _fontSize;
            set
            {
                if (TextNode is null)
                {
                    _fontSize = value;
                }
                else
                {
                    TextNode.FontSize = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets color of font used for text rendering. Default color is black.
        /// </summary>
        public Color Color
        {
            get => TextNode?.Color ?? _color;
            set
            {
                if (TextNode is null)
                {
                    _color = value;
                }
                else
                {
                    TextNode.Color = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets maximum width of layout box. Default value is 500 pixels.
        /// </summary>
        /// <remarks>
        ///     Text content is automatically wrapped to fit in layout box depending on <see cref="MaxWidth" /> of layout box.
        /// </remarks>
        /// <seealso cref="MaxHeight"/>
        /// <seealso cref="ClipToLayoutBox" />
        public double MaxWidth
        {
            get => TextNode?.MaxWidth ?? _maxWidth;
            set
            {
                if (TextNode is null)
                {
                    _maxWidth = value;
                }
                else
                {
                    TextNode.MaxWidth = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets maximum height of layout box. Default value is 500 pixels.
        /// </summary>
        /// <seealso cref="MaxWidth"/>
        /// <seealso cref="ClipToLayoutBox" />
        public double MaxHeight
        {
            get => TextNode?.MaxHeight ?? _maxHeight;
            set
            {
                if (TextNode is null)
                {
                    _maxHeight = value;
                }
                else
                {
                    TextNode.MaxHeight = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets alignment of text in paragraph, relative to the leading and trailing edge of the layout box.
        /// </summary>
        public TextAlignment TextAlignment
        {
            get => TextNode?.TextAlignment ?? _textAlignment;
            set
            {
                if (TextNode is null)
                {
                    _textAlignment = value;
                }
                else
                {
                    TextNode.TextAlignment = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets alignment option of a paragraph relative to the layout box's top and bottom edge.
        /// </summary>
        public ParagraphAlignment ParagraphAlignment
        {
            get => TextNode?.ParagraphAlignment ?? _paragraphAlignment;
            set
            {
                if (TextNode is null)
                {
                    _paragraphAlignment = value;
                }
                else
                {
                    TextNode.ParagraphAlignment = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets pivot point which defines origin for transformations during the rendering. It is defined relative to
        ///     layout box.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Layout coordinates are based on the origin in upper left corner of layout box being a (0,0) point and with
        ///         axes going x-right and y-down.
        ///     </para>
        ///     <para>
        ///         For example a <see cref="Pivot" /> equal (0,0) makes rendering origin aligned with upper left corner of layout
        ///         box.
        ///     </para>
        ///     <para>
        ///         For example a <see cref="Pivot" /> equal half of the <see cref="MaxWidth" /> and
        ///         half of the <see cref="MaxHeight" /> makes rendering origin aligned with center of layout box.
        ///     </para>
        /// </remarks>
        public Vector2 Pivot
        {
            get => TextNode?.Pivot ?? _pivot;
            set
            {
                if (TextNode is null)
                {
                    _pivot = value;
                }
                else
                {
                    TextNode.Pivot = value;
                }
            }
        }

        /// <summary>
        ///     Gets or sets value indicating whether rendered text should be clipped to the layout rectangle. Default value is
        ///     <c>false</c>.
        /// </summary>
        /// <seealso cref="MaxWidth"/>
        /// <seealso cref="MaxHeight"/>
        public bool ClipToLayoutBox
        {
            get => TextNode?.ClipToLayoutBox ?? _clipToLayoutBox;
            set
            {
                if (TextNode is null)
                {
                    _clipToLayoutBox = value;
                }
                else
                {
                    TextNode.ClipToLayoutBox = value;
                }
            }
        }

        /// <summary>
        ///     Gets overall metrics for the formatted text content.
        /// </summary>
        /// <remarks>
        ///     This property returns default value of <see cref="Rendering.TextMetrics" /> when
        ///     <see cref="TextRendererComponent" /> belongs to <see cref="Scene" /> that is not managed by rendering system.
        /// </remarks>
        public TextMetrics TextMetrics => TextNode?.Metrics ?? default;

        /// <summary>
        ///     Gets rectangle of layout box in local coordinate system, translated by <see cref="Pivot" /> so the pivot point is
        ///     at (0,0).
        /// </summary>
        /// <remarks>
        ///     This property returns default value of <see cref="AxisAlignedRectangle" /> when
        ///     <see cref="TextRendererComponent" /> belongs to <see cref="Scene" /> that is not managed by rendering system.
        /// </remarks>
        public AxisAlignedRectangle LayoutRectangle => TextNode?.LayoutRectangle ?? default;

        /// <summary>
        ///     Gets bounding rectangle of text content in local coordinate system, translated by <see cref="Pivot" /> so the pivot
        ///     point is at (0,0).
        /// </summary>
        /// <remarks>
        ///     This property returns default value of <see cref="AxisAlignedRectangle" /> when
        ///     <see cref="TextRendererComponent" /> belongs to <see cref="Scene" /> that is not managed by rendering system.
        /// </remarks>
        public AxisAlignedRectangle TextRectangle => TextNode?.TextRectangle ?? default;

        protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
            base.Serialize(writer, assetStore);
            writer.WriteString("Text", Text);
            writer.WriteString("FontFamilyName", FontFamilyName);
            writer.WriteDouble("FontSize", FontSize.Dips);
            writer.WriteColor("Color", Color);
            writer.WriteDouble("MaxWidth", MaxWidth);
            writer.WriteDouble("MaxHeight", MaxHeight);
            writer.WriteEnum("TextAlignment", TextAlignment);
            writer.WriteEnum("ParagraphAlignment", ParagraphAlignment);
            writer.WriteVector2("Pivot", Pivot);
            writer.WriteBool("ClipToLayoutBox", ClipToLayoutBox);
        }

        protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
            base.Deserialize(reader, assetStore);
            Text = reader.ReadString("Text") ?? throw new InvalidOperationException("Text cannot be null.");
            FontFamilyName = reader.ReadString("FontFamilyName") ?? throw new InvalidOperationException("FontFamilyName cannot be null.");
            FontSize = FontSize.FromDips(reader.ReadDouble("FontSize"));
            Color = reader.ReadColor("Color");
            MaxWidth = reader.ReadDouble("MaxWidth");
            MaxHeight = reader.ReadDouble("MaxHeight");
            TextAlignment = reader.ReadEnum<TextAlignment>("TextAlignment");
            ParagraphAlignment = reader.ReadEnum<ParagraphAlignment>("ParagraphAlignment");
            Pivot = reader.ReadVector2("Pivot");
            ClipToLayoutBox = reader.ReadBool("ClipToLayoutBox");
        }
    }

    internal sealed class TextRendererComponentFactory : ComponentFactory<TextRendererComponent>
    {
        protected override TextRendererComponent CreateComponent(Entity entity) => new(entity);
    }
}