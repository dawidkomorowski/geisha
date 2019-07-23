using Geisha.Common.Math;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input.Components;

namespace Geisha.TestGame.Behaviors
{
    [SerializableComponent]
    public class BoxMovementComponent : BehaviorComponent
    {
        [SerializableProperty]
        public double LinearVelocity { get; set; } = 250;

        [SerializableProperty]
        public double AngularVelocity { get; set; } = 1;

        public override void OnStart()
        {
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
            var transform = Entity.GetComponent<TransformComponent>();
            var input = Entity.GetComponent<InputComponent>();

            var movementVector = (transform.VectorY * input.GetAxisState("MoveUp")).Unit;
            var rotationVector = new Vector3(0, 0, -input.GetAxisState("MoveRight"));

            transform.Translation += movementVector * LinearVelocity * GameTime.FixedDeltaTime.TotalSeconds;
            transform.Rotation += rotationVector * AngularVelocity * GameTime.FixedDeltaTime.TotalSeconds;
        }
    }
}