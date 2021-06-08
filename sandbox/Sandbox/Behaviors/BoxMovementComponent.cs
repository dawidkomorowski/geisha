using System.Diagnostics;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input.Components;

namespace Sandbox.Behaviors
{
    internal sealed class BoxMovementComponent : BehaviorComponent
    {
        public double LinearVelocity { get; set; } = 250;
        public double AngularVelocity { get; set; } = 1;

        public override void OnStart()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");
            var input = Entity.GetComponent<InputComponent>();

            //input.BindAxis("MoveUp", value =>
            //{
            //    var movementVector = new Vector3(0, value, 0).Unit * Velocity;
            //    transform.Translation = transform.Translation + movementVector;
            //});
            //input.BindAxis("MoveRight", value =>
            //{
            //    var movementVector = new Vector3(value, 0, 0).Unit * Velocity;
            //    transform.Translation = transform.Translation + movementVector;
            //});
            input.BindAction("JetRotateRight", () => { });

            // TODO Common utils for interpolation?
            // TODO Enabled (entity or component property?)
            // TODO DeltaTime Smoothing
        }

        public override void OnFixedUpdate()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");
            var transform = Entity.GetComponent<Transform2DComponent>();
            var input = Entity.GetComponent<InputComponent>();

            var movementVector = (transform.VectorY * input.GetAxisState("MoveUp")).Unit;
            var rotation = -input.GetAxisState("MoveRight");

            transform.Translation += movementVector * LinearVelocity * GameTime.FixedDeltaTime.TotalSeconds;
            transform.Rotation += rotation * AngularVelocity * GameTime.FixedDeltaTime.TotalSeconds;
        }

        protected override void Serialize(IComponentDataWriter componentDataWriter, IAssetStore assetStore)
        {
            base.Serialize(componentDataWriter, assetStore);

            componentDataWriter.WriteDouble("LinearVelocity", LinearVelocity);
            componentDataWriter.WriteDouble("AngularVelocity", AngularVelocity);
        }

        protected override void Deserialize(IComponentDataReader componentDataReader, IAssetStore assetStore)
        {
            base.Deserialize(componentDataReader, assetStore);

            LinearVelocity = componentDataReader.ReadDouble("LinearVelocity");
            AngularVelocity = componentDataReader.ReadDouble("AngularVelocity");
        }
    }

    internal sealed class BoxMovementComponentFactory : ComponentFactory<BoxMovementComponent>
    {
        protected override BoxMovementComponent CreateComponent() => new BoxMovementComponent();
    }
}