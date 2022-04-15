using System;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    internal sealed class BehaviorSystem : IBehaviorSystem
    {
        public void ProcessBehaviorFixedUpdate()
        {
            PerformUpdate(behavior => behavior.OnFixedUpdate());
        }

        public void ProcessBehaviorUpdate(GameTime gameTime)
        {
            PerformUpdate(behavior => behavior.OnUpdate(gameTime));
        }

        private void PerformUpdate(Action<BehaviorComponent> updateAction)
        {
            // TODO This ToList() is needed for case of adding entity during iteration. There is no test for that.
            // TODO Also it will soon be reimplemented so it could be handled then.
            var entities = Enumerable.Empty<Entity>();
            foreach (var entity in entities)
            {
                if (entity.HasComponent<BehaviorComponent>())
                {
                    var behaviors = entity.GetComponents<BehaviorComponent>().ToList();
                    foreach (var behavior in behaviors)
                    {
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