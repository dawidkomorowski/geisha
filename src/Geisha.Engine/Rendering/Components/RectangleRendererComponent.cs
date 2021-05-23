using System;
using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Rectangle renderer component enables entity with rectangle rendering functionality.
    /// </summary>
    [ComponentId("Geisha.Engine.Rendering.RectangleRendererComponent")]
    public sealed class RectangleRendererComponent : Renderer2DComponent
    {
        /// <summary>
        ///     Dimension of rectangle. Rectangle has center at point (0,0) in local coordinate system.
        /// </summary>
        public Vector2 Dimension { get; set; }

        /// <summary>
        ///     Color of the rectangle.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        ///     Specifies whether to fill interior of rectangle or draw only border. If true interior is filled with color.
        /// </summary>
        public bool FillInterior { get; set; }

        protected internal override void Serialize(IComponentDataWriter componentDataWriter, IAssetStore assetStore)
        {
            componentDataWriter.WriteBool("Visible", Visible);
            componentDataWriter.WriteString("SortingLayerName", SortingLayerName);
            componentDataWriter.WriteInt("OrderInLayer", OrderInLayer);
            componentDataWriter.WriteVector2("Dimension", Dimension);
            componentDataWriter.WriteColor("Color", Color);
            componentDataWriter.WriteBool("FillInterior", FillInterior);
        }

        protected internal override void Deserialize(IComponentDataReader componentDataReader, IAssetStore assetStore)
        {
            Visible = componentDataReader.ReadBool("Visible");
            SortingLayerName = componentDataReader.ReadString("SortingLayerName") ??
                               throw new InvalidOperationException("SortingLayerName cannot be null.");
            OrderInLayer = componentDataReader.ReadInt("OrderInLayer");
            Dimension = componentDataReader.ReadVector2("Dimension");
            Color = componentDataReader.ReadColor("Color");
            FillInterior = componentDataReader.ReadBool("FillInterior");
        }
    }

    internal sealed class RectangleRendererComponentFactory : ComponentFactory<RectangleRendererComponent>
    {
        protected override RectangleRendererComponent CreateComponent() => new RectangleRendererComponent();
    }
}