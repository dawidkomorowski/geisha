using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.PhysicsEngine2D;

namespace Geisha.Engine.Physics.Systems;

internal sealed class PhysicsSystemState
{
    private readonly PhysicsScene2D _physicsScene2D;
    private readonly Dictionary<Entity, TrackedEntity> _trackedEntities = new();
    private readonly List<PhysicsBodyProxy> _physicsBodyProxies = new();

    public PhysicsSystemState(PhysicsScene2D physicsScene2D)
    {
        _physicsScene2D = physicsScene2D;
    }

    public IReadOnlyList<PhysicsBodyProxy> GetPhysicsBodyProxies() => _physicsBodyProxies;

    public void OnEntityParentChanged(Entity entity)
    {
        if (!_trackedEntities.TryGetValue(entity, out var trackedEntity))
        {
            return;
        }

        RemovePhysicsBody(trackedEntity);
        CreatePhysicsBody(trackedEntity);
        RemoveTrackedEntityIfNoLongerNeeded(trackedEntity);
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

        RemovePhysicsBody(trackedEntity);
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
        CreatePhysicsBody(trackedEntity);
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
        if (trackedEntity.PhysicsBodyProxy is not null) return;

        if (trackedEntity.IsStaticBody)
        {
            var proxy = PhysicsBodyProxy.CreateStatic(trackedEntity.Transform, trackedEntity.Collider);
            proxy.CreateInternalBody(_physicsScene2D);
            _physicsBodyProxies.Add(proxy);
            trackedEntity.PhysicsBodyProxy = proxy;
        }

        if (trackedEntity.IsKinematicBody)
        {
            var proxy = PhysicsBodyProxy.CreateKinematic(trackedEntity.Transform, trackedEntity.Collider, trackedEntity.KinematicBodyComponent);
            proxy.CreateInternalBody(_physicsScene2D);
            _physicsBodyProxies.Add(proxy);
            trackedEntity.PhysicsBodyProxy = proxy;
        }
    }

    private void RemovePhysicsBody(TrackedEntity trackedEntity)
    {
        if (trackedEntity.PhysicsBodyProxy is null) return;

        // TODO This implementation probably breaks changing kinematic into static and more.
        //if (trackedEntity.IsStaticBody || trackedEntity.IsKinematicBody) return;

        _physicsBodyProxies.Remove(trackedEntity.PhysicsBodyProxy);
        trackedEntity.PhysicsBodyProxy.Dispose();
        trackedEntity.PhysicsBodyProxy = null;
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

        public PhysicsBodyProxy? PhysicsBodyProxy { get; set; }

        [MemberNotNullWhen(true, nameof(Transform), nameof(Collider), nameof(KinematicBodyComponent))]
        public bool IsKinematicBody =>
            Transform is not null &&
            Collider is not null &&
            KinematicBodyComponent is not null &&
            Entity.IsRoot;

        [MemberNotNullWhen(true, nameof(Transform), nameof(Collider))]
        public bool IsStaticBody =>
            Transform is not null &&
            Collider is not null &&
            KinematicBodyComponent is null &&
            !Entity.Root.HasComponent<KinematicRigidBody2DComponent>();

        public bool ShouldBeRemoved => Transform is null && Collider is null && KinematicBodyComponent is null;
    }
}