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
        _physicsConfiguration = physicsConfiguration;
        _debugRenderer = debugRenderer;

        _physicsScene2D = new PhysicsScene2D();
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

        // TODO Some data could be synchronized on when accessing it instead of loop per frame.
        for (var i = 0; i < physicsBodyProxies.Count; i++)
        {
            var proxy = physicsBodyProxies[i];
            proxy.SynchronizeComponents();
        }
    }

    public void PreparePhysicsDebugInformation()
    {
        if (!_physicsConfiguration.RenderCollisionGeometry) return;

        for (var i = 0; i < _physicsScene2D.Bodies.Count; i++)
        {
            var body = _physicsScene2D.Bodies[i];
            var color = body.Contacts.Count > 0 ? Color.Red : Color.Green;
            if (body.IsCircleCollider)
            {
                _debugRenderer.DrawCircle(body.TransformedCircleCollider, color);
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

            foreach (var contact in body.Contacts)
            {
                for (var j = 0; j < contact.PointsCount; j++)
                {
                    _debugRenderer.DrawCircle(new Circle(contact.PointAt(j).WorldPosition, 3), Color.FromArgb(255, 255, 165, 0));
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
}