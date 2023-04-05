using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;

namespace Sandbox.Behaviors
{
    internal sealed class BoxMovementComponent : BehaviorComponent
    {
        public BoxMovementComponent(Entity entity) : base(entity)
        {
        }

        public double LinearVelocity { get; set; } = 250;
        public double AngularVelocity { get; set; } = 1;

        public override void OnStart()
        {
            var input = Entity.GetComponent<InputComponent>();
            input.BindAction("JetRotateRight", () =>
            {
                var entity = Scene.CreateEntity();
                var transform = entity.CreateComponent<Transform2DComponent>();
                var random = new Random();
                transform.Translation = new Vector2(random.Next(-1000, 1000), random.Next(-1000, 1000));
                var rectangleRenderer = entity.CreateComponent<RectangleRendererComponent>();
                rectangleRenderer.Dimension = new Vector2(200, 200);
                rectangleRenderer.FillInterior = true;
                rectangleRenderer.Color = Color.FromArgb(255, 150, 150, 150);
            });
        }

        public override void OnFixedUpdate()
        {
            var transform = Entity.GetComponent<Transform2DComponent>();
            var input = Entity.GetComponent<InputComponent>();

            var movementVector = (transform.VectorY * input.GetAxisState("MoveUp")).Unit;
            var rotation = -input.GetAxisState("MoveRight");

            transform.Translation += movementVector * LinearVelocity * GameTime.FixedDeltaTime.TotalSeconds;
            transform.Rotation += rotation * AngularVelocity * GameTime.FixedDeltaTime.TotalSeconds;
        }

        protected override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
        {
            base.Serialize(writer, assetStore);

            writer.WriteDouble("LinearVelocity", LinearVelocity);
            writer.WriteDouble("AngularVelocity", AngularVelocity);
        }

        protected override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
        {
            base.Deserialize(reader, assetStore);

            LinearVelocity = reader.ReadDouble("LinearVelocity");
            AngularVelocity = reader.ReadDouble("AngularVelocity");
        }
    }

    internal sealed class BoxMovementComponentFactory : ComponentFactory<BoxMovementComponent>
    {
        protected override BoxMovementComponent CreateComponent(Entity entity) => new(entity);
    }
}