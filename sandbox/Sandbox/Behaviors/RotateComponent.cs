using System;
using System.Diagnostics;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace Sandbox.Behaviors
{
    internal sealed class RotateComponent : BehaviorComponent
    {
        public double Velocity { get; set; } = Math.PI / 2;

        public override void OnFixedUpdate()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");
            var transform = Entity.GetComponent<Transform2DComponent>();
            transform.Rotation += Velocity * GameTime.FixedDeltaTime.TotalSeconds;
        }

        protected override void Serialize(IComponentDataWriter componentDataWriter, IAssetStore assetStore)
        {
            base.Serialize(componentDataWriter, assetStore);

            componentDataWriter.WriteDouble("Velocity", Velocity);
        }

        protected override void Deserialize(IComponentDataReader componentDataReader, IAssetStore assetStore)
        {
            base.Deserialize(componentDataReader, assetStore);

            Velocity = componentDataReader.ReadDouble("Velocity");
        }
    }

    internal sealed class RotateComponentFactory : ComponentFactory<RotateComponent>
    {
        protected override RotateComponent CreateComponent() => new RotateComponent();
    }
}