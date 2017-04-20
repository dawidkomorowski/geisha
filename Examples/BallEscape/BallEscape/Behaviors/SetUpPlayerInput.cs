using Geisha.Common.Geometry;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Input.Components;

namespace BallEscape.Behaviors
{
    public class SetUpPlayerInput : Behavior
    {
        public override void OnStart()
        {
            var inputComponent = Entity.GetComponent<InputComponent>();
            var movement = Entity.GetComponent<Movement>();

            inputComponent.BindAxis("MoveUp", scale => { movement.AddMovementInput(new Vector2(0, 1) * scale); });
            inputComponent.BindAxis("MoveRight", scale => { movement.AddMovementInput(new Vector2(1, 0) * scale); });
        }
    }
}