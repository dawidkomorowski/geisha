using System.Diagnostics;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;

namespace TestGame.Behaviors
{
    [SerializableComponent]
    public class MousePointerComponent : BehaviorComponent
    {
        public bool LeftButtonPressed { get; private set; }

        public override void OnFixedUpdate()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");
            var transform = Entity.GetComponent<TransformComponent>();
            var input = Entity.GetComponent<InputComponent>();
            LeftButtonPressed = input.HardwareInput.MouseInput.LeftButton;
            var mousePosition = input.HardwareInput.MouseInput.Position;

            Debug.Assert(Entity.Scene != null, "Entity.Scene != null");
            var cameraEntity = Entity.Scene.RootEntities.Single(e => e.HasComponent<CameraComponent>());
            var worldPoint = cameraEntity.ScreenPointTo2DWorldPoint(mousePosition);
            transform.Translation = transform.Translation.WithX(worldPoint.X).WithY(worldPoint.Y);
        }
    }
}