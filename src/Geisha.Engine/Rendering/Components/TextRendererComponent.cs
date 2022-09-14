using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Text renderer component enables entity with text rendering functionality.
    /// </summary>
    [ComponentId("Geisha.Engine.Rendering.TextRendererComponent")]
    public sealed class TextRendererComponent : Renderer2DComponent
    {
        internal TextRendererComponent(Entity entity) : base(entity)
        {
        }

        /// <summary>
        ///     Text content to be rendered.
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        ///     Size of font used for text rendering.
        /// </summary>
        public FontSize FontSize { get; set; }

        /// <summary>
        ///     Color of font used for text rendering.
        /// </summary>
        public Color Color { get; set; }

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
        protected override TextRendererComponent CreateComponent(Entity entity) => new TextRendererComponent(entity);
    }
}