using System;
using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;
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

        protected internal override void Serialize(IComponentDataWriter componentDataWriter, IAssetStore assetStore)
        {
            base.Serialize(componentDataWriter, assetStore);
            componentDataWriter.WriteString("Text", Text);
            componentDataWriter.WriteDouble("FontSize", FontSize.Points);
            componentDataWriter.WriteColor("Color", Color);
        }

        protected internal override void Deserialize(IComponentDataReader componentDataReader, IAssetStore assetStore)
        {
            base.Deserialize(componentDataReader, assetStore);
            Text = componentDataReader.ReadString("Text") ??
                   throw new InvalidOperationException("Text cannot be null.");
            FontSize = FontSize.FromPoints(componentDataReader.ReadDouble("FontSize"));
            Color = componentDataReader.ReadColor("Color");
        }
    }

    internal sealed class TextRendererComponentFactory : ComponentFactory<TextRendererComponent>
    {
        protected override TextRendererComponent CreateComponent() => new TextRendererComponent();
    }
}