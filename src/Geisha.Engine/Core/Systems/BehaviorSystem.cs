using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems
{
    internal sealed class BehaviorSystem : IBehaviorGameLoopStep, ISceneObserver
    {
        private readonly List<BehaviorComponent> _components = new List<BehaviorComponent>();
        private readonly List<BehaviorComponent> _componentsPendingToAdd = new List<BehaviorComponent>();
        private readonly List<BehaviorComponent> _componentsPendingToRemove = new List<BehaviorComponent>();

        #region Implementation of IBehaviorGameLoopStep

        public void ProcessBehaviorFixedUpdate()
        {
            PerformUpdate(behavior => behavior.OnFixedUpdate());
        }

        public void ProcessBehaviorUpdate(GameTime gameTime)
        {
            PerformUpdate(behavior => behavior.OnUpdate(gameTime));
        }

        #endregion

        #region Implementation of ISceneObserver

        public void OnEntityCreated(Entity entity)
        {
        }

        public void OnEntityRemoved(Entity entity)
        {
        }

        public void OnEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent)
        {
        }

        public void OnComponentCreated(Component component)
        {
            if (component is BehaviorComponent behaviorComponent)
            {
                _componentsPendingToAdd.Add(behaviorComponent);
            }
        }

        public void OnComponentRemoved(Component component)
        {
            if (component is BehaviorComponent behaviorComponent)
            {
                _componentsPendingToRemove.Add(behaviorComponent);
            }
        }

        #endregion

        private void PerformUpdate(Action<BehaviorComponent> updateAction)
        {
            _components.AddRange(_componentsPendingToAdd);
            _componentsPendingToAdd.Clear();

            foreach (var componentToRemove in _componentsPendingToRemove)
            {
                _components.Remove(componentToRemove);
            }

            _componentsPendingToRemove.Clear();

            foreach (var behaviorComponent in _components)
            {
                if (!behaviorComponent.Started)
                {
                    behaviorComponent.OnStart();
                    behaviorComponent.Started = true;
                }

                updateAction(behaviorComponent);
            }
        }
    }
}