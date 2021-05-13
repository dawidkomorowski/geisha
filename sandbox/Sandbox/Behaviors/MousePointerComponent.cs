using System.Diagnostics;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;

namespace Sandbox.Behaviors
{
    public class MousePointerComponent : BehaviorComponent
    {
        public override ComponentId ComponentId { get; } = new ComponentId("Sandbox.Behaviors.MousePointerComponent");

        public bool LeftButtonPressed { get; private set; }

        public override void OnFixedUpdate()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");
            var transform = Entity.GetComponent<Transform2DComponent>();
            var input = Entity.GetComponent<InputComponent>();
            LeftButtonPressed = input.HardwareInput.MouseInput.LeftButton;
            var mousePosition = input.HardwareInput.MouseInput.Position;

            Debug.Assert(Entity.Scene != null, "Entity.Scene != null");
            var cameraEntity = Entity.Scene.RootEntities.Single(e => e.HasComponent<CameraComponent>());
            transform.Translation = cameraEntity.ScreenPointTo2DWorldPoint(mousePosition);
        }
    }
}