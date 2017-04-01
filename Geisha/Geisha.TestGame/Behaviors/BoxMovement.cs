using System;
using Geisha.Common.Geometry;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Input.Components;

namespace Geisha.TestGame.Behaviors
{
    public class BoxMovement : Behavior
    {
        private bool _wasSetUp = false;

        public double Velocity { get; set; } = 250;

        public override void OnUpdate(double deltaTime)
        {
            var transform = Entity.GetComponent<Transform>();
            var input = Entity.GetComponent<InputComponent>();

            if (!_wasSetUp)
            {
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
                input.BindAction("JetRotateRight", () => { transform.Rotation += new Vector3(0, 0, -Math.PI / 8); });

                _wasSetUp = true;
            }

            // TODO Common utils for interpolation?
            // TODO Camera component?
            // TODO Visibility (renderer property visible)
            // TODO Enabled (entity or component property?)
            // TODO DeltaTime Smoothing
        }

        public override void OnFixedUpdate()
        {
            var transform = Entity.GetComponent<Transform>();
            var input = Entity.GetComponent<InputComponent>();

            var movementVector = new Vector3(input.GetAxisState("MoveRight"), input.GetAxisState("MoveUp"), 0).Unit;
            transform.Translation = transform.Translation + movementVector * Velocity * Constants.VelocityScale;
        }
    }
}