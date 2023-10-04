using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Rendering.Systems;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Rectangle renderer component enables entity with rectangle rendering functionality.
    /// </summary>
    [ComponentId("Geisha.Engine.Rendering.RectangleRendererComponent")]
    public sealed class RectangleRendererComponent : Renderer2DComponent
    {
        internal RectangleRendererComponent(Entity entity) : base(entity, new DetachedRectangleNode())
        {
        }

        internal IRectangleNode RectangleNode
        {
            get => (IRectangleNode)RenderNode;
            set => RenderNode = value;
        }

        /// <summary>
        ///     Dimensions of rectangle. Rectangle has center at point (0,0) in local coordinate system.
        /// </summary>
        public Vector2 Dimensions
        {
            get => RectangleNode.Dimensions;
            set => RectangleNode.Dimensions = value;
        }

        /// <summary>
        ///     Color of the rectangle.
        /// </summary>
        public Color Color
        {
            get => RectangleNode.Color;
            set => RectangleNode.Color = value;
        }

        /// <summary>
        ///     Specifies whether to fill interior of rectangle or draw only border. If <c>true</c> interior is filled with color.
        ///     Default is <c>false</c>.
        /// </summary>
        public bool FillInterior
        {
            get => RectangleNode.FillInterior;
            set => RectangleNode.FillInterior = value;
        }

        protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
            base.Serialize(writer, assetStore);
            writer.WriteVector2("Dimensions", Dimensions);
            writer.WriteColor("Color", Color);
            writer.WriteBool("FillInterior", FillInterior);
        }

        protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
            base.Deserialize(reader, assetStore);
            Dimensions = reader.ReadVector2("Dimensions");
            Color = reader.ReadColor("Color");
            FillInterior = reader.ReadBool("FillInterior");
        }
    }

    internal sealed class RectangleRendererComponentFactory : ComponentFactory<RectangleRendererComponent>
    {
        protected override RectangleRendererComponent CreateComponent(Entity entity) => new(entity);
    }
}