﻿using System;
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
    private readonly PhysicsState _physicsState;

    public PhysicsSystem(PhysicsConfiguration physicsConfiguration, IDebugRenderer debugRenderer)
    {
        _physicsConfiguration = physicsConfiguration;
        _debugRenderer = debugRenderer;

        _physicsScene2D = new PhysicsScene2D();
        _physicsState = new PhysicsState(_physicsScene2D);
    }

    #region Implementation of IPhysicsGameLoopStep

    public void ProcessPhysics()
    {
        var physicsBodyProxies = _physicsState.GetPhysicsBodyProxies();

        for (var i = 0; i < physicsBodyProxies.Count; i++)
        {
            var proxy = physicsBodyProxies[i];
            proxy.SynchronizeBody();
        }

        _physicsScene2D.Simulate(GameTime.FixedDeltaTime);

        for (var i = 0; i < physicsBodyProxies.Count; i++)
        {
            var proxy = physicsBodyProxies[i];
            proxy.SynchronizeComponents();
        }

        var staticBodies = _physicsState.GetStaticBodies();

        // TODO It could be updated on actual change instead of loop per frame.
        for (var i = 0; i < staticBodies.Count; i++)
        {
            var staticBody = staticBodies[i];
            staticBody.UpdateTransform();
        }

        var kinematicBodies = _physicsState.GetKinematicBodies();

        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var kinematicBody = kinematicBodies[i];
            kinematicBody.InitializeKinematicData();
        }

        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var kinematicBody = kinematicBodies[i];
            kinematicBody.UpdateTransform();
        }

        CollisionDetection.DetectCollisions(_physicsState);

        for (int i = 0; i < 6; i++)
        {
            ContactSolver.SolvePositionConstraints(kinematicBodies);
        }

        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var kinematicBody = kinematicBodies[i];
            kinematicBody.UpdateTransform();
        }
    }

    public void PreparePhysicsDebugInformation()
    {
        if (!_physicsConfiguration.RenderCollisionGeometry) return;

        foreach (var body in _physicsScene2D.Bodies)
        {
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
                //throw new InvalidOperationException($"Unknown collider component type: {kinematicBody.Collider.GetType()}.");
                throw new InvalidOperationException("Unknown collider component type.");
            }

            foreach (var contact in body.Contacts)
            {
                for (var i = 0; i < contact.PointsCount; i++)
                {
                    _debugRenderer.DrawCircle(new Circle(contact.PointAt(i).WorldPosition, 3), Color.FromArgb(255, 255, 165, 0));
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
        _physicsState.OnEntityParentChanged(entity);
    }

    public void OnComponentCreated(Component component)
    {
        switch (component)
        {
            case Transform2DComponent transform2DComponent:
                _physicsState.CreateStateFor(transform2DComponent);
                break;
            case Collider2DComponent collider2DComponent:
                _physicsState.CreateStateFor(collider2DComponent);
                break;
            case KinematicRigidBody2DComponent kinematicRigidBody2DComponent:
                _physicsState.CreateStateFor(kinematicRigidBody2DComponent);
                break;
        }
    }

    public void OnComponentRemoved(Component component)
    {
        switch (component)
        {
            case Transform2DComponent transform2DComponent:
                _physicsState.RemoveStateFor(transform2DComponent);
                break;
            case Collider2DComponent collider2DComponent:
                _physicsState.RemoveStateFor(collider2DComponent);
                break;
            case KinematicRigidBody2DComponent kinematicRigidBody2DComponent:
                _physicsState.RemoveStateFor(kinematicRigidBody2DComponent);
                break;
        }
    }

    #endregion
}