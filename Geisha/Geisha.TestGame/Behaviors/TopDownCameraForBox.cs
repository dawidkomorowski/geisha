using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Definition;

namespace Geisha.TestGame.Behaviors
{
    [ComponentDefinition]
    public class TopDownCameraForBox : Behavior
    {
        private Entity _box;

        public override void OnStart()
        {
            _box = Entity.Scene.AllEntities.Single(e => e.HasComponent<BoxMovement>());
            SetCameraTransformAsBoxTransform();
        }

        public override void OnFixedUpdate()
        {
            SetCameraTransformAsBoxTransform();
        }

        private void SetCameraTransformAsBoxTransform()
        {
            var transform = Entity.GetComponent<Transform>();
            var boxTransform = _box.GetComponent<Transform>();

            transform.Translation = boxTransform.Translation;
            transform.Rotation = boxTransform.Rotation;
        }
    }
}