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
    [ComponentId("Geisha.Engine.Rendering.TextRendererComponent")]
    public sealed class TextRendererComponent : Renderer2DComponent
    {
        private TextNode? _textNode;
        private string _text = string.Empty;

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
        public string FontFamilyName { get; set; } = "Consolas";

        /// <summary>
        ///     Size of font used for text rendering. Default size is 20 pixels.
        /// </summary>
        public FontSize FontSize { get; set; } = FontSize.FromDips(20);

        /// <summary>
        ///     Color of font used for text rendering. Default color is black.
        /// </summary>
        public Color Color { get; set; } = Color.Black;

        // TODO Update serialization with new properties
        protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
            base.Serialize(writer, assetStore);
            writer.WriteString("Text", Text);
            writer.WriteDouble("FontSize", FontSize.Points);
            writer.WriteColor("Color", Color);
        }

        protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
            base.Deserialize(reader, assetStore);
            Text = reader.ReadString("Text") ??
                   throw new InvalidOperationException("Text cannot be null.");
            FontSize = FontSize.FromPoints(reader.ReadDouble("FontSize"));
            Color = reader.ReadColor("Color");
        }
    }

    internal sealed class TextRendererComponentFactory : ComponentFactory<TextRendererComponent>
    {
        protected override TextRendererComponent CreateComponent(Entity entity) => new(entity);
    }
}