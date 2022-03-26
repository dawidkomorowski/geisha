using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;

namespace Sandbox.Behaviors
{
    internal sealed class MousePointerComponent : BehaviorComponent
    {
        public MousePointerComponent(Entity entity) : base(entity)
        {
        }

        public bool LeftButtonPressed { get; private set; }

        public override void OnFixedUpdate()
        {
            var transform = Entity.GetComponent<Transform2DComponent>();
            var input = Entity.GetComponent<InputComponent>();
            LeftButtonPressed = input.HardwareInput.MouseInput.LeftButton;
            var mousePosition = input.HardwareInput.MouseInput.Position;

            var cameraEntity = Scene.RootEntities.Single(e => e.HasComponent<CameraComponent>());
            transform.Translation = cameraEntity.ScreenPointTo2DWorldPoint(mousePosition);
        }
    }

    internal sealed class MousePointerComponentFactory : ComponentFactory<MousePointerComponent>
    {
        protected override MousePointerComponent CreateComponent(Entity entity) => new MousePointerComponent(entity);
    }
}