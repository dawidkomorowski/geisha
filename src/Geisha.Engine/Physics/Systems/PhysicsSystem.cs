using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.PhysicsEngine2D;

namespace Geisha.Engine.Physics.Systems;

internal sealed class PhysicsSystem : IPhysicsSystem, IPhysicsGameLoopStep, ISceneObserver
{
    private readonly ITimeSystem _timeSystem;
    private readonly IDebugRenderer _debugRenderer;
    private readonly PhysicsScene2D_V2 _physicsScene2D;
    private readonly PhysicsSystemState _physicsSystemState;

    public PhysicsSystem(PhysicsConfiguration physicsConfiguration, ITimeSystem timeSystem, IDebugRenderer debugRenderer)
    {
        if (physicsConfiguration.Substeps < 1)
        {
            throw new ArgumentException($"Configuration is invalid. {nameof(PhysicsConfiguration.Substeps)} must be at least 1.", nameof(physicsConfiguration));
        }

        if (physicsConfiguration.VelocityIterations < 1)
        {
            throw new ArgumentException($"Configuration is invalid. {nameof(PhysicsConfiguration.VelocityIterations)} must be at least 1.",
                nameof(physicsConfiguration));
        }

        if (physicsConfiguration.PositionIterations < 1)
        {
            throw new ArgumentException($"Configuration is invalid. {nameof(PhysicsConfiguration.PositionIterations)} must be at least 1.",
                nameof(physicsConfiguration));
        }

        if (physicsConfiguration.PenetrationTolerance < 0)
        {
            throw new ArgumentException($"Configuration is invalid. {nameof(PhysicsConfiguration.PenetrationTolerance)} must be at least 0.",
                nameof(physicsConfiguration));
        }

        if (physicsConfiguration.TileSize.Width <= 0 || physicsConfiguration.TileSize.Height <= 0)
        {
            throw new ArgumentException($"Configuration is invalid. {nameof(PhysicsConfiguration.TileSize)} must have positive dimensions.",
                nameof(physicsConfiguration));
        }

        EnableDebugRendering = physicsConfiguration.EnableDebugRendering;

        _timeSystem = timeSystem;
        _debugRenderer = debugRenderer;

        PhysicsScene2D = new PhysicsScene2D(physicsConfiguration.TileSize)
        {
            Substeps = physicsConfiguration.Substeps,
            VelocityIterations = physicsConfiguration.VelocityIterations,
            PositionIterations = physicsConfiguration.PositionIterations,
            PenetrationTolerance = physicsConfiguration.PenetrationTolerance
        };

        var sceneDefinition = new PhysicsScene2DDefinition
        {
            TileSize = physicsConfiguration.TileSize,
            Substeps = physicsConfiguration.Substeps,
            VelocityIterations = physicsConfiguration.VelocityIterations,
            PositionIterations = physicsConfiguration.PositionIterations,
            PenetrationTolerance = physicsConfiguration.PenetrationTolerance
        };

        _physicsScene2D = PhysicsScene2D_V2.Create(sceneDefinition);

        _physicsSystemState = new PhysicsSystemState(PhysicsScene2D, _physicsScene2D);
    }

    public PhysicsScene2D PhysicsScene2D { get; }

    // TODO: Should it stay this way?
    internal RigidBody2D_V2 FindInternalBody(Entity entity)
    {
        var proxies = _physicsSystemState.GetPhysicsBodyProxies();
        foreach (var proxy in proxies)
        {
            if (proxy.Entity == entity)
            {
                return proxy.RigidBody;
            }
        }

        throw new ArgumentException("Internal body not found for specified entity.");
    }

    #region Implementation of IPhysicsSystem

    public bool EnableDebugRendering { get; set; }

    public void SynchronizePhysicsState()
    {
        var physicsBodyProxies = _physicsSystemState.GetPhysicsBodyProxies();

        foreach (var proxy in physicsBodyProxies)
        {
            proxy.SynchronizeBody();
        }
    }

    public int QueryPoint(in Vector2 point, Span<Collider2DComponent> colliders)
    {
        var collidersArray = ArrayPool<Collider2DComponent>.Shared.Rent(colliders.Length);
        var queryHandler = new ColliderArrayQueryHandler(_physicsSystemState, collidersArray, colliders.Length);
        _physicsScene2D.QueryPoint(point, ref queryHandler);

        for (var i = 0; i < queryHandler.Count; i++)
        {
            colliders[i] = collidersArray[i];
        }

        ArrayPool<Collider2DComponent>.Shared.Return(collidersArray, true);

        return queryHandler.Count;
    }

    public int QueryPoint(in Vector2 point, List<Collider2DComponent> colliders)
    {
        colliders.Clear();
        var queryHandler = new ColliderListQueryHandler(_physicsSystemState, colliders);
        _physicsScene2D.QueryPoint(point, ref queryHandler);
        return queryHandler.Count;
    }

    public ReadOnlySpan<Collider2DComponent> QueryPointAsSpan(in Vector2 point, Span<Collider2DComponent> colliders)
    {
        var written = QueryPoint(point, colliders);
        return colliders.Slice(0, written);
    }

    public ReadOnlySpan<Collider2DComponent> QueryPointAsSpan(in Vector2 point, List<Collider2DComponent> colliders)
    {
        var written = QueryPoint(point, colliders);
        return CollectionsMarshal.AsSpan(colliders).Slice(0, written);
    }

    public int QueryBounds(in AxisAlignedRectangle axisAlignedRectangle, Span<Collider2DComponent> colliders)
    {
        var collidersArray = ArrayPool<Collider2DComponent>.Shared.Rent(colliders.Length);
        var queryHandler = new ColliderArrayQueryHandler(_physicsSystemState, collidersArray, colliders.Length);
        _physicsScene2D.QueryBounds(axisAlignedRectangle, ref queryHandler);

        for (var i = 0; i < queryHandler.Count; i++)
        {
            colliders[i] = collidersArray[i];
        }

        ArrayPool<Collider2DComponent>.Shared.Return(collidersArray, true);

        return queryHandler.Count;
    }

    public int QueryBounds(in AxisAlignedRectangle axisAlignedRectangle, List<Collider2DComponent> colliders)
    {
        colliders.Clear();
        var queryHandler = new ColliderListQueryHandler(_physicsSystemState, colliders);
        _physicsScene2D.QueryBounds(axisAlignedRectangle, ref queryHandler);
        return queryHandler.Count;
    }

    public ReadOnlySpan<Collider2DComponent> QueryBoundsAsSpan(in AxisAlignedRectangle axisAlignedRectangle, Span<Collider2DComponent> colliders)
    {
        var written = QueryBounds(axisAlignedRectangle, colliders);
        return colliders.Slice(0, written);
    }

    public ReadOnlySpan<Collider2DComponent> QueryBoundsAsSpan(in AxisAlignedRectangle axisAlignedRectangle, List<Collider2DComponent> colliders)
    {
        var written = QueryBounds(axisAlignedRectangle, colliders);
        return CollectionsMarshal.AsSpan(colliders).Slice(0, written);
    }

    public int QueryOverlap(in AxisAlignedRectangle axisAlignedRectangle, Span<Collider2DComponent> colliders)
    {
        var collidersArray = ArrayPool<Collider2DComponent>.Shared.Rent(colliders.Length);
        var queryHandler = new ColliderArrayQueryHandler(_physicsSystemState, collidersArray, colliders.Length);
        _physicsScene2D.QueryOverlap(axisAlignedRectangle, ref queryHandler);

        for (var i = 0; i < queryHandler.Count; i++)
        {
            colliders[i] = collidersArray[i];
        }

        ArrayPool<Collider2DComponent>.Shared.Return(collidersArray, true);

        return queryHandler.Count;
    }

    public int QueryOverlap(in AxisAlignedRectangle axisAlignedRectangle, List<Collider2DComponent> colliders)
    {
        colliders.Clear();
        var queryHandler = new ColliderListQueryHandler(_physicsSystemState, colliders);
        _physicsScene2D.QueryOverlap(axisAlignedRectangle, ref queryHandler);
        return queryHandler.Count;
    }

    public ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in AxisAlignedRectangle axisAlignedRectangle, Span<Collider2DComponent> colliders)
    {
        var written = QueryOverlap(axisAlignedRectangle, colliders);
        return colliders.Slice(0, written);
    }

    public ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in AxisAlignedRectangle axisAlignedRectangle, List<Collider2DComponent> colliders)
    {
        var written = QueryOverlap(axisAlignedRectangle, colliders);
        return CollectionsMarshal.AsSpan(colliders).Slice(0, written);
    }

    public int QueryOverlap(in Circle circle, Span<Collider2DComponent> colliders)
    {
        var collidersArray = ArrayPool<Collider2DComponent>.Shared.Rent(colliders.Length);
        var queryHandler = new ColliderArrayQueryHandler(_physicsSystemState, collidersArray, colliders.Length);
        _physicsScene2D.QueryOverlap(circle, ref queryHandler);

        for (var i = 0; i < queryHandler.Count; i++)
        {
            colliders[i] = collidersArray[i];
        }

        ArrayPool<Collider2DComponent>.Shared.Return(collidersArray, true);

        return queryHandler.Count;
    }

    public int QueryOverlap(in Circle circle, List<Collider2DComponent> colliders)
    {
        colliders.Clear();
        var queryHandler = new ColliderListQueryHandler(_physicsSystemState, colliders);
        _physicsScene2D.QueryOverlap(circle, ref queryHandler);
        return queryHandler.Count;
    }

    public ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in Circle circle, Span<Collider2DComponent> colliders)
    {
        var written = QueryOverlap(circle, colliders);
        return colliders.Slice(0, written);
    }

    public ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in Circle circle, List<Collider2DComponent> colliders)
    {
        var written = QueryOverlap(circle, colliders);
        return CollectionsMarshal.AsSpan(colliders).Slice(0, written);
    }

    public int QueryOverlap(in Rectangle rectangle, Span<Collider2DComponent> colliders)
    {
        var collidersArray = ArrayPool<Collider2DComponent>.Shared.Rent(colliders.Length);
        var queryHandler = new ColliderArrayQueryHandler(_physicsSystemState, collidersArray, colliders.Length);
        _physicsScene2D.QueryOverlap(rectangle, ref queryHandler);

        for (var i = 0; i < queryHandler.Count; i++)
        {
            colliders[i] = collidersArray[i];
        }

        ArrayPool<Collider2DComponent>.Shared.Return(collidersArray, true);

        return queryHandler.Count;
    }

    public int QueryOverlap(in Rectangle rectangle, List<Collider2DComponent> colliders)
    {
        colliders.Clear();
        var queryHandler = new ColliderListQueryHandler(_physicsSystemState, colliders);
        _physicsScene2D.QueryOverlap(rectangle, ref queryHandler);
        return queryHandler.Count;
    }

    public ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in Rectangle rectangle, Span<Collider2DComponent> colliders)
    {
        var written = QueryOverlap(rectangle, colliders);
        return colliders.Slice(0, written);
    }

    public ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in Rectangle rectangle, List<Collider2DComponent> colliders)
    {
        var written = QueryOverlap(rectangle, colliders);
        return CollectionsMarshal.AsSpan(colliders).Slice(0, written);
    }

    #endregion

    #region Implementation of IPhysicsGameLoopStep

    public void ProcessPhysics()
    {
        var physicsBodyProxies = _physicsSystemState.GetPhysicsBodyProxies();

        foreach (var proxy in physicsBodyProxies)
        {
            proxy.SynchronizeBody();
        }

        _physicsScene2D.Simulate(_timeSystem.FixedDeltaTime);

        foreach (var proxy in physicsBodyProxies)
        {
            proxy.SynchronizeComponents();
        }

        InvokeEventCallbacks();
    }

    public void PreparePhysicsDebugInformation()
    {
        if (!EnableDebugRendering) return;

        var staticBodyColor = Color.Green;
        var kinematicBodyColor = Color.Blue;
        var contactPointColor = Color.FromArgb(255, 255, 165, 0);
        var contactNormalColor = Color.Black;
        var disabledCollisionDetectionBodyColor = Color.Gray;

        Span<Vector2> points = stackalloc Vector2[2];

        foreach (var body in PhysicsScene2D.Bodies)
        {
            var color = body.Type switch
            {
                BodyType.Static => staticBodyColor,
                BodyType.Kinematic => kinematicBodyColor,
                _ => throw new InvalidOperationException("Unsupported body type.")
            };

            if (!body.EnableCollisionDetection)
            {
                color = disabledCollisionDetectionBodyColor;
            }

            switch (body.ColliderType)
            {
                case ColliderType.Circle:
                {
                    _debugRenderer.DrawCircle(body.TransformedCircleCollider, color);

                    points[0] = Vector2.Zero;
                    points[1] = points[0] + Vector2.UnitX * body.TransformedCircleCollider.Radius;
                    var transform = new Transform2D(body.Position, body.Rotation, Vector2.One);
                    _debugRenderer.DrawRectangle(new AxisAlignedRectangle(points), color, transform.ToMatrix());

                    break;
                }
                case ColliderType.Rectangle:
                {
                    var rectangle = new AxisAlignedRectangle(body.RectangleColliderSize);
                    var transform = new Transform2D(body.Position, body.Rotation, Vector2.One);
                    _debugRenderer.DrawRectangle(rectangle, color, transform.ToMatrix());

                    break;
                }
                case ColliderType.Tile:
                {
                    var rectangle = new AxisAlignedRectangle(body.RectangleColliderSize);
                    var transform = new Transform2D(body.Position, 0, Vector2.One);
                    _debugRenderer.DrawRectangle(rectangle, color, transform.ToMatrix());

                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        foreach (var body in PhysicsScene2D.Bodies)
        {
            if (body.Type is not BodyType.Kinematic) continue;

            foreach (var contact in body.Contacts)
            {
                for (var j = 0; j < contact.ContactPoints.Count; j++)
                {
                    // Drawing contacts based on body dimensions to make it scale between different sizes.
                    // Otherwise, it either is too big or too small in different contexts (unit tests, sandbox).
                    // It should be improved in scope of https://github.com/dawidkomorowski/geisha/issues/562.
                    _debugRenderer.DrawCircle(new Circle(contact.ContactPoints[j].WorldPosition, body.BoundingRectangle.Width / 20d),
                        contactPointColor);

                    var normalLen = body.BoundingRectangle.Width / 2d;
                    var normalRect = new AxisAlignedRectangle(normalLen / 2d, 0, normalLen, 0 / 10d);
                    var sign = Math.Sign(-contact.CollisionNormal.Cross(Vector2.UnitX));
                    var normalRot = contact.CollisionNormal.Angle(Vector2.UnitX) * (sign == 0 ? 1 : sign);
                    _debugRenderer.DrawRectangle(normalRect, contactNormalColor,
                        Matrix3x3.CreateTRS(contact.ContactPoints[j].WorldPosition, normalRot, Vector2.One));
                }
            }
        }
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
        _physicsSystemState.OnEntityParentChanged(entity);
    }

    public void OnComponentCreated(Component component)
    {
        switch (component)
        {
            case Transform2DComponent transform2DComponent:
                _physicsSystemState.CreateStateFor(transform2DComponent);
                break;
            case Collider2DComponent collider2DComponent:
                _physicsSystemState.CreateStateFor(collider2DComponent);
                break;
            case KinematicRigidBody2DComponent kinematicRigidBody2DComponent:
                _physicsSystemState.CreateStateFor(kinematicRigidBody2DComponent);
                break;
        }
    }

    public void OnComponentRemoved(Component component)
    {
        switch (component)
        {
            case Transform2DComponent transform2DComponent:
                _physicsSystemState.RemoveStateFor(transform2DComponent);
                break;
            case Collider2DComponent collider2DComponent:
                _physicsSystemState.RemoveStateFor(collider2DComponent);
                break;
            case KinematicRigidBody2DComponent kinematicRigidBody2DComponent:
                _physicsSystemState.RemoveStateFor(kinematicRigidBody2DComponent);
                break;
        }
    }

    #endregion

    private void InvokeEventCallbacks()
    {
        foreach (var sensorOverlapEvent in _physicsScene2D.GetSensorOverlapEvents())
        {
            var proxy1 = _physicsSystemState.GetProxyById(sensorOverlapEvent.SensorId);
            var proxy2 = _physicsSystemState.GetProxyById(sensorOverlapEvent.VisitorId);

            var collider1 = proxy1.Collider;
            var collider2 = proxy2.Collider;

            switch (sensorOverlapEvent.Type)
            {
                case SensorOverlapEvent.EventType.Begin:
                    collider1.OnOverlapBegin?.Invoke(collider2);
                    collider2.OnOverlapBegin?.Invoke(collider1);
                    break;
                case SensorOverlapEvent.EventType.End:
                    collider1.OnOverlapEnd?.Invoke(collider2);
                    collider2.OnOverlapEnd?.Invoke(collider1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    // TODO: Use ColliderSpanQueryHandler instead when migrated to .NET 9 (C# 13) -> it allows ref structs to implement interfaces.
    //       Then the span based query handler can implement IRigidBodyQueryHandler.
    //       It will allow more memory friendly implementation of span based queries without accidental allocations and data copying.
    // TODO: When available, feature "ref fields" could be used to create query handler translating from RigidBodyId -> RigidBody
    //       so the public PhysicsEngine2D API would not need to operate on raw ID.
    private struct ColliderArrayQueryHandler : IRigidBodyIdQueryHandler
    {
        private readonly PhysicsSystemState _physicsSystemState;
        private readonly Collider2DComponent[] _colliders;
        private readonly int _maxCount;

        public ColliderArrayQueryHandler(PhysicsSystemState physicsSystemState, Collider2DComponent[] colliders, int maxCount)
        {
            _physicsSystemState = physicsSystemState;
            _colliders = colliders;
            _maxCount = maxCount;
            Count = 0;
        }

        public int Count { get; private set; }

        public bool Handle(RigidBodyId id)
        {
            if (Count >= _maxCount)
            {
                return false;
            }

            var proxy = _physicsSystemState.GetProxyById(id);
            _colliders[Count++] = proxy.Collider;

            return true;
        }
    }

    //private ref struct ColliderSpanQueryHandler : IRigidBodyQueryHandler
    //{
    //    private readonly Span<Collider2DComponent> _colliders;

    //    public ColliderSpanQueryHandler(Span<Collider2DComponent> colliders)
    //    {
    //        _colliders = colliders;
    //        Count = 0;
    //    }

    //    public int Count { get; private set; }

    //    public bool Handle(RigidBody2D body)
    //    {
    //        if (Count >= _colliders.Length)
    //        {
    //            return false;
    //        }

    //        var proxy = body.Proxy;
    //        Debug.Assert(proxy is not null);
    //        _colliders[Count++] = proxy.Collider;

    //        return true;
    //    }
    //}

    private struct ColliderListQueryHandler : IRigidBodyIdQueryHandler
    {
        private readonly PhysicsSystemState _physicsSystemState;
        private readonly List<Collider2DComponent> _colliders;

        public ColliderListQueryHandler(PhysicsSystemState physicsSystemState, List<Collider2DComponent> colliders)
        {
            _physicsSystemState = physicsSystemState;
            _colliders = colliders;
            Count = 0;
        }

        public int Count { get; private set; }

        public bool Handle(RigidBodyId id)
        {
            var proxy = _physicsSystemState.GetProxyById(id);
            _colliders.Add(proxy.Collider);

            Count++;

            return true;
        }
    }
}