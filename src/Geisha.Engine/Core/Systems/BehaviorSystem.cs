using System;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    internal sealed class BehaviorSystem : IBehaviorSystem
    {
        public void ProcessBehaviorFixedUpdate(Scene scene)
        {
            PerformUpdate(scene, behavior => behavior.OnFixedUpdate());
        }

        public void ProcessBehaviorUpdate(Scene scene, GameTime gameTime)
        {
            PerformUpdate(scene, behavior => behavior.OnUpdate(gameTime));
        }

        private void PerformUpdate(Scene scene, Action<BehaviorComponent> updateAction)
        {
            foreach (var entity in scene.AllEntities)
            {
                if (entity.HasComponent<BehaviorComponent>())
                {
                    var behaviors = entity.GetComponents<BehaviorComponent>().ToList();
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