using System;
using System.ComponentModel.Composition;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    [Export(typeof(ISystem))]
    public class BehaviorSystem : ISystem
    {
        public int Priority { get; set; } = 1;
        public UpdateMode UpdateMode { get; set; } = UpdateMode.Both;

        public void Update(Scene scene, double deltaTime)
        {
            PerformUpdate(scene, behavior => behavior.OnUpdate(deltaTime));
        }

        public void FixedUpdate(Scene scene)
        {
            PerformUpdate(scene, behavior => behavior.OnFixedUpdate());
        }

        private void PerformUpdate(Scene scene, Action<Behavior> updateAction)
        {
            foreach (var entity in scene.AllEntities.ToList())
            {
                if (entity.HasComponent<Behavior>())
                {
                    var behaviors = entity.GetComponents<Behavior>();
                    foreach (var behavior in behaviors)
                    {
                        behavior.Entity = entity;

                        if (!behavior.Started)
                        {
                            behavior.OnStart();
                            behavior.Started = true;
                        }

                        updateAction(behavior);
                    }
                }
            }
        }
    }
}