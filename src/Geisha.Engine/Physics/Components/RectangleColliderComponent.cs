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
        /// <summary>
        ///     Dimension of rectangle. Rectangle has center at point (0,0) in local coordinate system.
        /// </summary>
        public Vector2 Dimension { get; set; }

        protected internal override void Serialize(IComponentDataWriter componentDataWriter, IAssetStore assetStore)
        {
            base.Serialize(componentDataWriter, assetStore);
            componentDataWriter.WriteVector2("Dimension", Dimension);
        }

        protected internal override void Deserialize(IComponentDataReader componentDataReader, IAssetStore assetStore)
        {
            base.Deserialize(componentDataReader, assetStore);
            Dimension = componentDataReader.ReadVector2("Dimension");
        }
    }

    internal sealed class RectangleColliderComponentFactory : ComponentFactory<RectangleColliderComponent>
    {
        protected override RectangleColliderComponent CreateComponent() => new RectangleColliderComponent();
    }
}