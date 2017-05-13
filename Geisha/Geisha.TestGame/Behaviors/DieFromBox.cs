using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.TestGame.Behaviors
{
    public class DieFromBox : Behavior
    {
        private Scene _scene;
        private Entity _box;

        public double DieDistance { get; set; } = 100;

        public override void OnStart()
        {
            _scene = Entity.Scene;
            _box = _scene.AllEntities.Single(e => e.HasComponent<BoxMovement>());
        }

        public override void OnFixedUpdate()
        {
            var boxTransform = _box.GetComponent<Transform>();
            var transform = Entity.GetComponent<Transform>();

            if (transform.Translation.Distance(boxTransform.Translation) < DieDistance)
            {
                Entity.Destroy();
            }
        }
    }
}