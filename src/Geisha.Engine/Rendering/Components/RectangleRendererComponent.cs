﻿using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
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
        internal RectangleRendererComponent(Entity entity) : base(entity)
        {
        }

        /// <summary>
        ///     Dimension of rectangle. Rectangle has center at point (0,0) in local coordinate system.
        /// </summary>
        // TODO Dimension or Dimensions? Typically dimensions is used to describe the size of something.
        public Vector2 Dimension { get; set; }

        /// <summary>
        ///     Color of the rectangle.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        ///     Specifies whether to fill interior of rectangle or draw only border. If <c>true</c> interior is filled with color.
        ///     Default is <c>false</c>.
        /// </summary>
        public bool FillInterior { get; set; }

        protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
            base.Serialize(writer, assetStore);
            writer.WriteVector2("Dimension", Dimension);
            writer.WriteColor("Color", Color);
            writer.WriteBool("FillInterior", FillInterior);
        }

        protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
            base.Deserialize(reader, assetStore);
            Dimension = reader.ReadVector2("Dimension");
            Color = reader.ReadColor("Color");
            FillInterior = reader.ReadBool("FillInterior");
        }
    }

    internal sealed class RectangleRendererComponentFactory : ComponentFactory<RectangleRendererComponent>
    {
        protected override RectangleRendererComponent CreateComponent(Entity entity) => new RectangleRendererComponent(entity);
    }
}