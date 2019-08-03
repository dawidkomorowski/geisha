using System.Linq;
using Geisha.Common.Math;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Rendering.Components;

namespace Geisha.TestGame.Behaviors
{
    [SerializableComponent]
    public class MousePointerComponent : BehaviorComponent
    {
        public bool LeftButtonPressed { get; private set; }

        public override void OnFixedUpdate()
        {
            var transform = Entity.GetComponent<TransformComponent>();
            var input = Entity.GetComponent<InputComponent>();
            LeftButtonPressed = input.HardwareInput.MouseInput.LeftButton;
            var mousePosition = input.HardwareInput.MouseInput.Position;

            var cameraEntity = Entity.Scene.RootEntities.Single(e => e.HasComponent<CameraComponent>());
            var worldPoint = ScreenPointTo2DWorldPoint(cameraEntity, mousePosition);
            transform.Translation = transform.Translation.WithX(worldPoint.X).WithY(worldPoint.Y);
        }

        private static Vector2 ScreenPointTo2DWorldPoint(Entity cameraEntity, Vector2 screenPoint)
        {
            const double screenWidth = 1280.0;
            const double screenHeight = 720.0;
            var cameraTransform = cameraEntity.GetComponent<TransformComponent>();
            return (cameraTransform.Create2DTransformationMatrix() *
                    screenPoint.WithX(screenPoint.X - screenWidth / 2).WithY(-(screenPoint.Y - screenHeight / 2)).Homogeneous).ToVector2();
        }
    }
}