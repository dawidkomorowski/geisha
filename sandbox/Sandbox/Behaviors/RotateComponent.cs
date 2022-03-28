using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Sandbox.Behaviors
{
    internal sealed class RotateComponent : BehaviorComponent
    {
        public RotateComponent(Entity entity) : base(entity)
        {
        }

        public double Velocity { get; set; } = Math.PI / 2;

        public override void OnFixedUpdate()
        {
            var transform = Entity.GetComponent<Transform2DComponent>();
            transform.Rotation += Velocity * GameTime.FixedDeltaTime.TotalSeconds;
        }

        protected override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
            base.Serialize(writer, assetStore);

            writer.WriteDouble("Velocity", Velocity);
        }

        protected override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
            base.Deserialize(reader, assetStore);

            Velocity = reader.ReadDouble("Velocity");
        }
    }

    internal sealed class RotateComponentFactory : ComponentFactory<RotateComponent>
    {
        protected override RotateComponent CreateComponent(Entity entity) => new RotateComponent(entity);
    }
}