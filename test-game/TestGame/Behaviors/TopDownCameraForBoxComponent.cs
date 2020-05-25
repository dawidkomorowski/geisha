using System.Diagnostics;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;

namespace TestGame.Behaviors
{
    [SerializableComponent]
    public class TopDownCameraForBoxComponent : BehaviorComponent
    {
        private Entity _box = null!;

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
            var transform = Entity.GetComponent<TransformComponent>();
            var boxTransform = _box.GetComponent<TransformComponent>();

            transform.Translation = boxTransform.Translation;
            transform.Rotation = boxTransform.Rotation;
        }
    }
}