using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.PhysicsEngine2D;

namespace Geisha.Engine.Physics.Systems;

internal sealed class PhysicsState
{
    private readonly PhysicsScene2D _physicsScene2D;
    private readonly Dictionary<Entity, TrackedEntity> _trackedEntities = new();
    private readonly List<StaticBody> _staticBodies = new();
    private readonly List<KinematicBody> _kinematicBodies = new();

    public PhysicsState(PhysicsScene2D physicsScene2D)
    {
        _physicsScene2D = physicsScene2D;
    }

    public IReadOnlyList<StaticBody> GetStaticBodies() => _staticBodies;
    public IReadOnlyList<KinematicBody> GetKinematicBodies() => _kinematicBodies;

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
        if (trackedEntity.IsStaticBody && trackedEntity.StaticBody is null)
        {
            var staticBody = new StaticBody(trackedEntity.Transform, trackedEntity.Collider);
            _staticBodies.Add(staticBody);
            trackedEntity.StaticBody = staticBody;

            var proxy = PhysicsBodyProxy.CreateStatic(trackedEntity.Transform, trackedEntity.Collider);
            proxy.CreateInternalBody(_physicsScene2D);
            trackedEntity.PhysicsBodyProxy = proxy;
        }

        if (trackedEntity.IsKinematicBody && trackedEntity.KinematicBody is null)
        {
            var kinematicBody = new KinematicBody(trackedEntity.Transform, trackedEntity.Collider, trackedEntity.KinematicBodyComponent);
            _kinematicBodies.Add(kinematicBody);
            trackedEntity.KinematicBody = kinematicBody;

            var proxy = PhysicsBodyProxy.CreateKinematic(trackedEntity.Transform, trackedEntity.Collider, trackedEntity.KinematicBodyComponent);
            proxy.CreateInternalBody(_physicsScene2D);
            trackedEntity.PhysicsBodyProxy = proxy;
        }
    }

    private void RemovePhysicsBody(TrackedEntity trackedEntity)
    {
        if (!trackedEntity.IsStaticBody && trackedEntity.StaticBody is not null)
        {
            _staticBodies.Remove(trackedEntity.StaticBody);
            trackedEntity.StaticBody.Dispose();
            trackedEntity.StaticBody = null;

            trackedEntity.PhysicsBodyProxy.Dispose();
            trackedEntity.PhysicsBodyProxy = null;
        }

        if (!trackedEntity.IsKinematicBody && trackedEntity.KinematicBody is not null)
        {
            _kinematicBodies.Remove(trackedEntity.KinematicBody);
            trackedEntity.KinematicBody.Dispose();
            trackedEntity.KinematicBody = null;

            trackedEntity.PhysicsBodyProxy.Dispose();
            trackedEntity.PhysicsBodyProxy = null;
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
        public StaticBody? StaticBody { get; set; }
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

internal sealed class PhysicsBodyProxy : IDisposable
{
    private RigidBody2D? _body;

    private PhysicsBodyProxy(Transform2DComponent transform, Collider2DComponent collider, KinematicRigidBody2DComponent? kinematicBodyComponent)
    {
        Transform = transform;
        Collider = collider;
        KinematicBodyComponent = kinematicBodyComponent;
    }

    public static PhysicsBodyProxy CreateStatic(Transform2DComponent transform, Collider2DComponent collider)
    {
        return new PhysicsBodyProxy(transform, collider, null);
    }

    public static PhysicsBodyProxy CreateKinematic(Transform2DComponent transform, Collider2DComponent collider,
        KinematicRigidBody2DComponent? kinematicBodyComponent)
    {
        return new PhysicsBodyProxy(transform, collider, kinematicBodyComponent);
    }

    public Entity Entity => Transform.Entity;
    public Transform2DComponent Transform { get; }
    public Collider2DComponent Collider { get; }
    public KinematicRigidBody2DComponent? KinematicBodyComponent { get; }

    public void CreateInternalBody(PhysicsScene2D physicsScene2D)
    {
        Debug.Assert(_body == null, "_body == null");
        _body = physicsScene2D.CreateBody(BodyType.Static, new Circle());
    }

    public void Dispose()
    {
        Debug.Assert(_body != null, "_body != null");
        _body.Scene.RemoveBody(_body);
    }
}