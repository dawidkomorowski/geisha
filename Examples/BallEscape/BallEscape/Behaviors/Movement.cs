using Geisha.Common.Geometry;
using Geisha.Engine.Core.Components;

namespace BallEscape.Behaviors
{
    public class Movement : Behavior
    {
        private Vector2 _movementVector;
        public double Speed { get; set; } = 1;

        public override void OnFixedUpdate()
        {
            var transform = Entity.GetComponent<Transform>();
            var velocityVector = _movementVector.Unit * Speed;
            transform.Translation += new Vector3(velocityVector.X, velocityVector.Y, 0);
            _movementVector = Vector2.Zero;
        }

        public void AddMovementInput(Vector2 worldDirection)
        {
            _movementVector += worldDirection;
        }
    }
}