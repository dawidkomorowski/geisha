using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Geisha.Engine.Physics.Components
{
    /// <summary>
    ///     2D collider component in shape of a circle.
    /// </summary>
    [ComponentId("Geisha.Engine.Physics.CircleColliderComponent")]
    public sealed class CircleColliderComponent : Collider2DComponent
    {
        /// <summary>
        ///     Radius of circle.
        /// </summary>
        public double Radius { get; set; }

        protected internal override void Serialize(IComponentDataWriter componentDataWriter, IAssetStore assetStore)
        {
            base.Serialize(componentDataWriter, assetStore);
            componentDataWriter.WriteDouble("Radius", Radius);
        }

        protected internal override void Deserialize(IComponentDataReader componentDataReader, IAssetStore assetStore)
        {
            base.Deserialize(componentDataReader, assetStore);
            Radius = componentDataReader.ReadDouble("Radius");
        }
    }

    internal sealed class CircleColliderComponentFactory : ComponentFactory<CircleColliderComponent>
    {
        protected override CircleColliderComponent CreateComponent() => new CircleColliderComponent();
    }
}