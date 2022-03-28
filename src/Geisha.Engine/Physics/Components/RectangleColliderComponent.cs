using Geisha.Common.Math;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Physics.Components
{
    /// <summary>
    ///     2D collider component in shape of a rectangle.
    /// </summary>
    [ComponentId("Geisha.Engine.Physics.RectangleColliderComponent")]
    public sealed class RectangleColliderComponent : Collider2DComponent
    {
        internal RectangleColliderComponent(Entity entity) : base(entity)
        {
        }

        /// <summary>
        ///     Dimension of rectangle. Rectangle has center at point (0,0) in local coordinate system.
        /// </summary>
        // TODO Dimension or Dimensions? Typically dimensions is used to describe the size of something.
        public Vector2 Dimension { get; set; }

        protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
            base.Serialize(writer, assetStore);
            writer.WriteVector2("Dimension", Dimension);
        }

        protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
            base.Deserialize(reader, assetStore);
            Dimension = reader.ReadVector2("Dimension");
        }
    }

    internal sealed class RectangleColliderComponentFactory : ComponentFactory<RectangleColliderComponent>
    {
        protected override RectangleColliderComponent CreateComponent(Entity entity) => new RectangleColliderComponent(entity);
    }
}