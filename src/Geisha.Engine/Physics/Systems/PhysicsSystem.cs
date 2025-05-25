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

// TODO Collision Mask/Filter/Group?
// TODO Quad Tree optimization / Broad Phase?
internal sealed class PhysicsSystem : IPhysicsGameLoopStep, ISceneObserver
{
    private readonly PhysicsConfiguration _physicsConfiguration;
    private readonly IDebugRenderer _debugRenderer;
    private readonly PhysicsScene2D _physicsScene2D;
    private readonly PhysicsSystemState _physicsSystemState;

    public PhysicsSystem(PhysicsConfiguration physicsConfiguration, IDebugRenderer debugRenderer)
    {
        if (physicsConfiguration.Substeps < 1)
        {
            throw new ArgumentException("Configuration is invalid. Substeps must be at least 1.", nameof(physicsConfiguration));
        }

        _physicsConfiguration = physicsConfiguration;
        _debugRenderer = debugRenderer;

        _physicsScene2D = new PhysicsScene2D
        {
            Substeps = _physicsConfiguration.Substeps
        };

        _physicsSystemState = new PhysicsSystemState(_physicsScene2D);
    }

    #region Implementation of IPhysicsGameLoopStep

    public void ProcessPhysics()
    {
        var physicsBodyProxies = _physicsSystemState.GetPhysicsBodyProxies();

        // TODO Some data could be synchronized on actual change instead of loop per frame.
        for (var i = 0; i < physicsBodyProxies.Count; i++)
        {
            var proxy = physicsBodyProxies[i];
            proxy.SynchronizeBody();
        }

        _physicsScene2D.Simulate(GameTime.FixedDeltaTime);

        // TODO Some data could be synchronized only when accessing it instead of loop per frame.
        for (var i = 0; i < physicsBodyProxies.Count; i++)
        {
            var proxy = physicsBodyProxies[i];
            proxy.SynchronizeComponents();
        }
    }

    public void PreparePhysicsDebugInformation()
    {
        if (!_physicsConfiguration.RenderCollisionGeometry) return;

        Span<Vector2> points = stackalloc Vector2[2];

        for (var i = 0; i < _physicsScene2D.Bodies.Count; i++)
        {
            var body = _physicsScene2D.Bodies[i];
            var color = body.Type switch
            {
                BodyType.Static => Color.Green,
                BodyType.Kinematic => Color.Blue,
                _ => throw new InvalidOperationException("Unsupported body type.")
            };

            if (body.IsCircleCollider)
            {
                _debugRenderer.DrawCircle(body.TransformedCircleCollider, color);

                // TODO: It is a poor way of drawing lines. Extend debug renderer to support lines.
                points[0] = Vector2.Zero;
                points[1] = points[0] + Vector2.UnitX * body.TransformedCircleCollider.Radius;
                var transform = new Transform2D(body.Position, body.Rotation, Vector2.One);
                _debugRenderer.DrawRectangle(new AxisAlignedRectangle(points), color, transform.ToMatrix());
            }
            else if (body.IsRectangleCollider)
            {
                var rectangle = new AxisAlignedRectangle(body.RectangleCollider.Dimensions);
                var transform = new Transform2D(body.Position, body.Rotation, Vector2.One);
                _debugRenderer.DrawRectangle(rectangle, color, transform.ToMatrix());
            }
            else
            {
                throw new InvalidOperationException("Unknown collider component type.");
            }
        }

        for (var i = 0; i < _physicsScene2D.Bodies.Count; i++)
        {
            var body = _physicsScene2D.Bodies[i];
            if (body.Type is not BodyType.Kinematic) continue;

            foreach (var contact in body.Contacts)
            {
                for (var j = 0; j < contact.ContactPoints.Count; j++)
                {
                    // TODO Drawing contacts based on body dimensions to make it scale between different sizes.
                    // Otherwise, it either is too big or too small in different contexts (unit tests, sandbox).
                    // It should be improved in scope of https://github.com/dawidkomorowski/geisha/issues/562.
                    _debugRenderer.DrawCircle(new Circle(contact.ContactPoints[j].WorldPosition, body.BoundingRectangle.Width / 20d),
                        Color.FromArgb(255, 255, 165, 0));

                    var normalLen = body.BoundingRectangle.Width / 2d;
                    var normalRect = new AxisAlignedRectangle(normalLen / 2d, 0, normalLen, 0 / 10d);
                    // TODO Introduce Vector2.Angle func in Range [-PI, PI]?
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

    // TODO This method is a workaround for the issue: https://github.com/dawidkomorowski/geisha/issues/563
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