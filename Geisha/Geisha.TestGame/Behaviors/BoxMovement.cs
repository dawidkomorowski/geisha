using Geisha.Common.Math;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Definition;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;

namespace Geisha.TestGame.Behaviors
{
    [ComponentDefinition]
    public class BoxMovement : Behavior
    {
        private bool _flag = true;
        private double _originalScaleX;

        [PropertyDefinition]
        public double LinearVelocity { get; set; } = 250;

        [PropertyDefinition]
        public double AngularVelocity { get; set; } = 1;

        public override void OnStart()
        {
            var transform = Entity.GetComponent<Transform>();
            var input = Entity.GetComponent<InputComponent>();

            _originalScaleX = transform.Scale.X;

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
            input.BindAction("JetRotateRight", () =>
            {
                _flag = !_flag;
                //transform.Rotation += new Vector3(0, 0, Math.PI / 8);
                //transform.Translation = Vector3.Zero;
                transform.Scale = transform.Scale.WithX(_flag ? _originalScaleX : _originalScaleX * 2);

                foreach (var entity in Entity.Scene.AllEntities)
                {
                    if (entity.HasComponent<RendererBase>())
                    {
                        foreach (var rendererBase in entity.GetComponents<RendererBase>())
                        {
                            //rendererBase.Visible = !rendererBase.Visible;
                        }
                    }
                }
            });

            // TODO Common utils for interpolation?
            // TODO Enabled (entity or component property?)
            // TODO DeltaTime Smoothing
        }

        public override void OnFixedUpdate()
        {
            var transform = Entity.GetComponent<Transform>();
            var input = Entity.GetComponent<InputComponent>();

            //var movementVector = new Vector3(input.GetAxisState("MoveRight"), input.GetAxisState("MoveUp"), 0).Unit;
            var movementVector = (transform.VectorY * input.GetAxisState("MoveUp")).Unit;
            transform.Translation = transform.Translation + movementVector * LinearVelocity * GameTime.FixedDeltaTime.TotalSeconds;

            var rotationVector = new Vector3(0, 0, -input.GetAxisState("MoveRight") * AngularVelocity * GameTime.FixedDeltaTime.TotalSeconds);
            transform.Rotation = transform.Rotation + rotationVector;
        }
    }
}