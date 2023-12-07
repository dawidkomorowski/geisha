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
    private readonly List<KinematicBody> _kinematicBodies = new();

    public IReadOnlyList<KinematicBody> GetKinematicBodies() => _kinematicBodies;

    public void OnEntityParentChanged(Entity entity)
    {
        if (!_trackedEntities.TryGetValue(entity, out var trackedEntity))
        {
            return;
        }

        RemovePhysicsBody(trackedEntity);
        CreatePhysicsBody(trackedEntity);
    }

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

    public void CreateStateFor(KinematicRigidBody2DComponent kinematicRigidBody2DComponent)
    {
        var trackedEntity = GetOrCreateTrackedEntity(kinematicRigidBody2DComponent.Entity);

        if (trackedEntity.KinematicBodyComponent is not null)
        {
            throw new InvalidOperationException("Only single kinematic body component per entity is supported.");
        }

        trackedEntity.KinematicBodyComponent = kinematicRigidBody2DComponent;

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

    public void RemoveStateFor(KinematicRigidBody2DComponent kinematicRigidBody2DComponent)
    {
        var trackedEntity = _trackedEntities[kinematicRigidBody2DComponent.Entity];
        trackedEntity.KinematicBodyComponent = null;

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
        if (trackedEntity.IsKinematicBody && trackedEntity.KinematicBody is null)
        {
            var kinematicBody = new KinematicBody(trackedEntity.Transform, trackedEntity.Collider);
            _kinematicBodies.Add(kinematicBody);
            trackedEntity.KinematicBody = kinematicBody;
        }
    }

    private void RemovePhysicsBody(TrackedEntity trackedEntity)
    {
        if (!trackedEntity.IsKinematicBody && trackedEntity.KinematicBody is not null)
        {
            _kinematicBodies.Remove(trackedEntity.KinematicBody);
            trackedEntity.KinematicBody = null;
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
        public KinematicRigidBody2DComponent? KinematicBodyComponent { get; set; }

        public KinematicBody? KinematicBody { get; set; }

        [MemberNotNullWhen(true, nameof(Transform), nameof(Collider), nameof(KinematicBodyComponent))]
        public bool IsKinematicBody => Transform is not null && Collider is not null && KinematicBodyComponent is not null && Entity.IsRoot;

        public bool ShouldBeRemoved => Transform is null && Collider is null && KinematicBodyComponent is null;
    }
}