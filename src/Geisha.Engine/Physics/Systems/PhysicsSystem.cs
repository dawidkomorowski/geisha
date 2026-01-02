using System;
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
    private readonly IDebugRenderer _debugRenderer;
    private readonly PhysicsSystemState _physicsSystemState;

    public PhysicsSystem(PhysicsConfiguration physicsConfiguration, IDebugRenderer debugRenderer)
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

        _debugRenderer = debugRenderer;

        PhysicsScene2D = new PhysicsScene2D(physicsConfiguration.TileSize)
        {
            Substeps = physicsConfiguration.Substeps,
            VelocityIterations = physicsConfiguration.VelocityIterations,
            PositionIterations = physicsConfiguration.PositionIterations,
            PenetrationTolerance = physicsConfiguration.PenetrationTolerance
        };

        _physicsSystemState = new PhysicsSystemState(PhysicsScene2D);
    }

    public PhysicsScene2D PhysicsScene2D { get; }

    #region Implementation of IPhysicsSystem

    public bool EnableDebugRendering { get; set; }

    #endregion

    #region Implementation of IPhysicsGameLoopStep

    public void ProcessPhysics()
    {
        var physicsBodyProxies = _physicsSystemState.GetPhysicsBodyProxies();

        for (var i = 0; i < physicsBodyProxies.Count; i++)
        {
            var proxy = physicsBodyProxies[i];
            proxy.SynchronizeBody();
        }

        PhysicsScene2D.Simulate(GameTime.FixedDeltaTime);

        for (var i = 0; i < physicsBodyProxies.Count; i++)
        {
            var proxy = physicsBodyProxies[i];
            proxy.SynchronizeComponents();
        }
    }

    public void PreparePhysicsDebugInformation()
    {
        if (!EnableDebugRendering) return;

        Span<Vector2> points = stackalloc Vector2[2];

        for (var i = 0; i < PhysicsScene2D.Bodies.Count; i++)
        {
            var body = PhysicsScene2D.Bodies[i];
            var color = body.Type switch
            {
                BodyType.Static => Color.Green,
                BodyType.Kinematic => Color.Blue,
                _ => throw new InvalidOperationException("Unsupported body type.")
            };

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

        for (var i = 0; i < PhysicsScene2D.Bodies.Count; i++)
        {
            var body = PhysicsScene2D.Bodies[i];
            if (body.Type is not BodyType.Kinematic) continue;

            foreach (var contact in body.Contacts)
            {
                for (var j = 0; j < contact.ContactPoints.Count; j++)
                {
                    // Drawing contacts based on body dimensions to make it scale between different sizes.
                    // Otherwise, it either is too big or too small in different contexts (unit tests, sandbox).
                    // It should be improved in scope of https://github.com/dawidkomorowski/geisha/issues/562.
                    _debugRenderer.DrawCircle(new Circle(contact.ContactPoints[j].WorldPosition, body.BoundingRectangle.Width / 20d),
                        Color.FromArgb(255, 255, 165, 0));

                    var normalLen = body.BoundingRectangle.Width / 2d;
                    var normalRect = new AxisAlignedRectangle(normalLen / 2d, 0, normalLen, 0 / 10d);
                    var sign = Math.Sign(-contact.CollisionNormal.Cross(Vector2.UnitX));
                    var normalRot = contact.CollisionNormal.Angle(Vector2.UnitX) * (sign == 0 ? 1 : sign);
                    _debugRenderer.DrawRectangle(normalRect, Color.Black,
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

    // This method is a workaround for the issue: https://github.com/dawidkomorowski/geisha/issues/563
    public void SynchronizeBodies()
    {
        var physicsBodyProxies = _physicsSystemState.GetPhysicsBodyProxies();

        for (var i = 0; i < physicsBodyProxies.Count; i++)
        {
            var proxy = physicsBodyProxies[i];
            proxy.SynchronizeBody();
        }
    }
}