using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;

namespace Geisha.Engine.Physics.Systems;

internal sealed class PhysicsState
{
    private readonly Dictionary<Entity, TrackedEntity> _trackedEntities = new();
    private readonly List<PhysicsBody> _physicsBodies = new();

    public IReadOnlyList<PhysicsBody> GetPhysicsBodies() => _physicsBodies;

    public void CreateStateFor(Transform2DComponent transform2DComponent)
    {
        var trackedEntity = GetOrCreateTrackedEntity(transform2DComponent.Entity);

        if (trackedEntity.Transform is not null)
        {
            throw new InvalidOperationException("Only single transform component per entity is supported.");
        }

        trackedEntity.Transform = transform2DComponent;

        CreatePhysicsBody(trackedEntity);
    }

    public void CreateStateFor(Collider2DComponent collider2DComponent)
    {
        var trackedEntity = GetOrCreateTrackedEntity(collider2DComponent.Entity);

        if (trackedEntity.Collider is not null)
        {
            throw new InvalidOperationException("Only single collider component per entity is supported.");
        }

        trackedEntity.Collider = collider2DComponent;

        CreatePhysicsBody(trackedEntity);
    }

    public void RemoveStateFor(Transform2DComponent transform2DComponent)
    {
        var trackedEntity = _trackedEntities[transform2DComponent.Entity];
        trackedEntity.Transform = null;

        RemovePhysicsBody(trackedEntity);
        RemoveTrackedEntityIfNoLongerNeeded(trackedEntity);
    }

    public void RemoveStateFor(Collider2DComponent collider2DComponent)
    {
        var trackedEntity = _trackedEntities[collider2DComponent.Entity];
        trackedEntity.Collider = null;

        RemovePhysicsBody(trackedEntity);
        RemoveTrackedEntityIfNoLongerNeeded(trackedEntity);
    }

    private TrackedEntity GetOrCreateTrackedEntity(Entity entity)
    {
        if (_trackedEntities.TryGetValue(entity, out var trackedEntity))
        {
            return trackedEntity;
        }

        trackedEntity = new TrackedEntity(entity);
        _trackedEntities.Add(entity, trackedEntity);
        return trackedEntity;
    }

    private void RemoveTrackedEntityIfNoLongerNeeded(TrackedEntity trackedEntity)
    {
        if (trackedEntity.ShouldBeRemoved)
        {
            _trackedEntities.Remove(trackedEntity.Entity);
        }
    }

    private void CreatePhysicsBody(TrackedEntity trackedEntity)
    {
        if (trackedEntity.IsPhysicsBody && trackedEntity.PhysicsBody is null)
        {
            var physicsBody = new PhysicsBody(trackedEntity.Transform, trackedEntity.Collider);
            _physicsBodies.Add(physicsBody);
            trackedEntity.PhysicsBody = physicsBody;
        }
    }

    private void RemovePhysicsBody(TrackedEntity trackedEntity)
    {
        if (!trackedEntity.IsPhysicsBody && trackedEntity.PhysicsBody is not null)
        {
            _physicsBodies.Remove(trackedEntity.PhysicsBody);
            trackedEntity.PhysicsBody = null;
        }
    }

    private sealed class TrackedEntity
    {
        public TrackedEntity(Entity entity)
        {
            Entity = entity;
        }

        public Entity Entity { get; }

        public Transform2DComponent? Transform { get; set; }
        public Collider2DComponent? Collider { get; set; }

        public PhysicsBody? PhysicsBody { get; set; }

        [MemberNotNullWhen(true, nameof(Transform), nameof(Collider))]
        public bool IsPhysicsBody => Transform is not null && Collider is not null;

        public bool ShouldBeRemoved => Transform is null && Collider is null;
    }
}