using System;
using System.Collections.Generic;
using System.Diagnostics;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Core.Systems;

internal sealed class BehaviorSystem : IBehaviorGameLoopStep, ISceneObserver
{
    private readonly List<BehaviorComponent> _components = new();
    private readonly List<BehaviorComponent> _componentsPendingToAdd = new();
    private readonly List<BehaviorComponent> _componentsPendingToRemove = new();

    #region Implementation of IBehaviorGameLoopStep

    public void ProcessBehaviorFixedUpdate()
    {
        PerformUpdate(UpdateAction.FixedUpdate);
    }

    public void ProcessBehaviorUpdate(GameTime gameTime)
    {
        PerformUpdate(UpdateAction.Update, gameTime);
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

    private void PerformUpdate(UpdateAction updateAction, GameTime? gameTime = null)
    {
        _components.AddRange(_componentsPendingToAdd);
        _componentsPendingToAdd.Clear();

        // Cannot use foreach here because OnRemove may modify the list.
        for (var i = 0; i < _componentsPendingToRemove.Count; i++)
        {
            var componentToRemove = _componentsPendingToRemove[i];
            componentToRemove.OnRemove();
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

            switch (updateAction)
            {
                case UpdateAction.Update:
                    Debug.Assert(gameTime is not null, "gameTime is not null");
                    behaviorComponent.OnUpdate(gameTime.Value);
                    break;
                case UpdateAction.FixedUpdate:
                    behaviorComponent.OnFixedUpdate();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(updateAction), updateAction, $"Unexpected update action: {updateAction}.");
            }
        }
    }

    private enum UpdateAction
    {
        Update,
        FixedUpdate
    }
}