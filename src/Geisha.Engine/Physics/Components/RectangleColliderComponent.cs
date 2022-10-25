using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
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
        ///     Dimensions of rectangle. Rectangle has center at point (0,0) in local coordinate system.
        /// </summary>
        public Vector2 Dimensions { get; set; }

        protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
            base.Serialize(writer, assetStore);
            writer.WriteVector2("Dimensions", Dimensions);
        }

        protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
            base.Deserialize(reader, assetStore);
            Dimensions = reader.ReadVector2("Dimensions");
        }
    }

    internal sealed class RectangleColliderComponentFactory : ComponentFactory<RectangleColliderComponent>
    {
        protected override RectangleColliderComponent CreateComponent(Entity entity) => new(entity);
    }
}