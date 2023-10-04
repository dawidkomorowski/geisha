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
        internal TextRendererComponent(Entity entity) : base(entity, new DetachedTextNode())
        {
            Text = string.Empty;
            FontFamilyName = "Consolas";
            FontSize = FontSize.FromDips(20);
            Color = Color.Black;
            MaxWidth = 500;
            MaxHeight = 500;
            TextAlignment = TextAlignment.Leading;
            ParagraphAlignment = ParagraphAlignment.Near;
        }

        internal ITextNode TextNode
        {
            get => (ITextNode)RenderNode;
            set => RenderNode = value;
        }

        /// <summary>
        ///     Gets or sets text content to be rendered.
        /// </summary>
        public string Text
        {
            get => TextNode.Text;
            set => TextNode.Text = value;
        }

        /// <summary>
        ///     Gets or sets font family name used for text rendering. Default is Consolas.
        /// </summary>
        /// <remarks>This property allows to use fonts installed in operating system by specifying font family name.</remarks>
        public string FontFamilyName
        {
            get => TextNode.FontFamilyName;
            set => TextNode.FontFamilyName = value;
        }

        /// <summary>
        ///     Gets or sets size of font used for text rendering. Default size is 20 pixels.
        /// </summary>
        public FontSize FontSize
        {
            get => TextNode.FontSize;
            set => TextNode.FontSize = value;
        }

        /// <summary>
        ///     Gets or sets color of font used for text rendering. Default color is black.
        /// </summary>
        public Color Color
        {
            get => TextNode.Color;
            set => TextNode.Color = value;
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
            get => TextNode.MaxWidth;
            set => TextNode.MaxWidth = value;
        }

        /// <summary>
        ///     Gets or sets maximum height of layout box. Default value is 500 pixels.
        /// </summary>
        /// <seealso cref="MaxWidth"/>
        /// <seealso cref="ClipToLayoutBox" />
        public double MaxHeight
        {
            get => TextNode.MaxHeight;
            set => TextNode.MaxHeight = value;
        }

        /// <summary>
        ///     Gets or sets alignment of text in paragraph, relative to the leading and trailing edge of the layout box.
        /// </summary>
        public TextAlignment TextAlignment
        {
            get => TextNode.TextAlignment;
            set => TextNode.TextAlignment = value;
        }

        /// <summary>
        ///     Gets or sets alignment option of a paragraph relative to the layout box's top and bottom edge.
        /// </summary>
        public ParagraphAlignment ParagraphAlignment
        {
            get => TextNode.ParagraphAlignment;
            set => TextNode.ParagraphAlignment = value;
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
            get => TextNode.Pivot;
            set => TextNode.Pivot = value;
        }

        /// <summary>
        ///     Gets or sets value indicating whether rendered text should be clipped to the layout rectangle. Default value is
        ///     <c>false</c>.
        /// </summary>
        /// <seealso cref="MaxWidth"/>
        /// <seealso cref="MaxHeight"/>
        public bool ClipToLayoutBox
        {
            get => TextNode.ClipToLayoutBox;
            set => TextNode.ClipToLayoutBox = value;
        }

        /// <summary>
        ///     Gets overall metrics for the formatted text content.
        /// </summary>
        /// <remarks>
        ///     This property returns default value of <see cref="Rendering.TextMetrics" /> when
        ///     <see cref="TextRendererComponent" /> is not managed by rendering system.
        /// </remarks>
        /// <seealso cref="Renderer2DComponent.IsManagedByRenderingSystem"/>
        public TextMetrics TextMetrics => TextNode.Metrics;

        /// <summary>
        ///     Gets rectangle of layout box in local coordinate system, translated by <see cref="Pivot" /> so the pivot point is
        ///     at (0,0).
        /// </summary>
        /// <remarks>
        ///     This property returns default value of <see cref="AxisAlignedRectangle" /> when
        ///     <see cref="TextRendererComponent" /> is not managed by rendering system.
        /// </remarks>
        /// <seealso cref="Renderer2DComponent.IsManagedByRenderingSystem" />
        public AxisAlignedRectangle LayoutRectangle => TextNode.LayoutRectangle;

        /// <summary>
        ///     Gets bounding rectangle of text content in local coordinate system, translated by <see cref="Pivot" /> so the pivot
        ///     point is at (0,0).
        /// </summary>
        /// <remarks>
        ///     This property returns default value of <see cref="AxisAlignedRectangle" /> when
        ///     <see cref="TextRendererComponent" /> is not managed by rendering system.
        /// </remarks>
        /// <seealso cref="Renderer2DComponent.IsManagedByRenderingSystem" />
        public AxisAlignedRectangle TextRectangle => TextNode.TextRectangle;

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