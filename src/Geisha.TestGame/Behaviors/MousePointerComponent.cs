using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input.Components;

namespace Geisha.TestGame.Behaviors
{
    [SerializableComponent]
    public class MousePointerComponent : BehaviorComponent
    {
        public override void OnFixedUpdate()
        {
            var transform = Entity.GetComponent<TransformComponent>();
            var input = Entity.GetComponent<InputComponent>();
            var mousePosition = input.HardwareInput.MouseInput.Position;
            transform.Translation = transform.Translation.WithX(mousePosition.X).WithY(-mousePosition.Y);
        }
    }
}