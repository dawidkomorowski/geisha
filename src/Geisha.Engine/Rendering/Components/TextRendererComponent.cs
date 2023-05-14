using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Systems;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Text renderer component enables entity with text rendering functionality.
    /// </summary>
    [ComponentId("Geisha.Engine.Rendering.TextRendererComponent")]
    public sealed class TextRendererComponent : Renderer2DComponent
    {
        private TextNode? _textNode;
        private string _text = string.Empty;
        private string _fontFamilyName = "Consolas";
        private FontSize _fontSize = FontSize.FromDips(20);
        private double _maxWidth = 500;
        private double _maxHeight = 500;
        private TextAlignment _textAlignment = TextAlignment.Leading;
        private ParagraphAlignment _paragraphAlignment = ParagraphAlignment.Near;

        internal TextRendererComponent(Entity entity) : base(entity)
        {
        }

        internal TextNode? TextNode
        {
            get => _textNode;
            set
            {
                if (_textNode is not null && value is null)
                {
                    _text = _textNode.Text;
                    // TODO Create test for this scenario and add rest of properties.
                }

                _textNode = value;
            }
        }


        /// <summary>
        ///     Text content to be rendered.
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

        // TODO Add documentation.
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
        ///     Size of font used for text rendering. Default size is 20 pixels.
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
        ///     Color of font used for text rendering. Default color is black.
        /// </summary>
        public Color Color { get; set; } = Color.Black;

        // TODO Add documentation.
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

        // TODO Add documentation.
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

        // TODO Add documentation.
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

        // TODO Add documentation.
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

        // TODO Add documentation.
        public Vector2 Pivot { get; set; }

        protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
            base.Serialize(writer, assetStore);
            writer.WriteString("Text", Text);
            writer.WriteString("FontFamilyName", FontFamilyName);
            writer.WriteDouble("FontSize", FontSize.Points);
            writer.WriteColor("Color", Color);
            writer.WriteDouble("MaxWidth", MaxWidth);
            writer.WriteDouble("MaxHeight", MaxHeight);
            writer.WriteEnum("TextAlignment", TextAlignment);
            writer.WriteEnum("ParagraphAlignment", ParagraphAlignment);
            writer.WriteVector2("Pivot", Pivot);
        }

        protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
            base.Deserialize(reader, assetStore);
            Text = reader.ReadString("Text") ?? throw new InvalidOperationException("Text cannot be null.");
            FontFamilyName = reader.ReadString("FontFamilyName") ?? throw new InvalidOperationException("FontFamilyName cannot be null.");
            FontSize = FontSize.FromPoints(reader.ReadDouble("FontSize"));
            Color = reader.ReadColor("Color");
            MaxWidth = reader.ReadDouble("MaxWidth");
            MaxHeight = reader.ReadDouble("MaxHeight");
            TextAlignment = reader.ReadEnum<TextAlignment>("TextAlignment");
            ParagraphAlignment = reader.ReadEnum<ParagraphAlignment>("ParagraphAlignment");
            Pivot = reader.ReadVector2("Pivot");
        }
    }

    internal sealed class TextRendererComponentFactory : ComponentFactory<TextRendererComponent>
    {
        protected override TextRendererComponent CreateComponent(Entity entity) => new(entity);
    }
}