using System.Diagnostics;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Sandbox.Behaviors
{
    public class TopDownCameraForBoxComponent : BehaviorComponent
    {
        private Entity _box = null!;

        public override ComponentId ComponentId { get; } = new ComponentId("Sandbox.Behaviors.TopDownCameraForBoxComponent");

        public override void OnStart()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");
            Debug.Assert(Entity.Scene != null, "Entity.Scene != null");
            _box = Entity.Scene.AllEntities.Single(e => e.HasComponent<BoxMovementComponent>());
            SetCameraTransformAsBoxTransform();
        }

        public override void OnFixedUpdate()
        {
            SetCameraTransformAsBoxTransform();
        }

        private void SetCameraTransformAsBoxTransform()
        {
            Debug.Assert(Entity != null, nameof(Entity) + " != null");
            var transform = Entity.GetComponent<Transform2DComponent>();
            var boxTransform = _box.GetComponent<Transform2DComponent>();

            transform.Translation = boxTransform.Translation;
            transform.Rotation = boxTransform.Rotation;
        }
    }
}