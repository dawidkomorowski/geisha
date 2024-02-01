using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;

namespace Geisha.Engine.Physics.Systems;

// TODO Collision Mask/Filter/Group?
// TODO Quad Tree optimization / Broad Phase?
internal sealed class PhysicsSystem : IPhysicsGameLoopStep, ISceneObserver
{
    private readonly PhysicsConfiguration _physicsConfiguration;
    private readonly IDebugRenderer _debugRenderer;
    private readonly PhysicsState _physicsState = new();

    public PhysicsSystem(PhysicsConfiguration physicsConfiguration, IDebugRenderer debugRenderer)
    {
        _physicsConfiguration = physicsConfiguration;
        _debugRenderer = debugRenderer;
    }

    #region Implementation of IPhysicsGameLoopStep

    public void ProcessPhysics()
    {
        var deltaTimeSeconds = GameTime.FixedDeltaTimeSeconds;

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

        KinematicIntegrator.IntegrateKinematicMotion(_physicsState, deltaTimeSeconds);

        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var kinematicBody = kinematicBodies[i];
            kinematicBody.UpdateTransform();
        }

        CollisionDetection.DetectCollisions(_physicsState);

        for (int i = 0; i < 6; i++)
        {
            ContactConstraintSolver.Solve(kinematicBodies);
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

        foreach (var staticBody in _physicsState.GetStaticBodies())
        {
            var color = GetColor(staticBody.Collider.IsColliding);

            if (staticBody.IsCircleCollider)
            {
                _debugRenderer.DrawCircle(staticBody.TransformedCircle, color);
            }
            else if (staticBody.IsRectangleCollider)
            {
                var rectangle = new AxisAlignedRectangle(((RectangleColliderComponent)staticBody.Collider).Dimensions);
                _debugRenderer.DrawRectangle(rectangle, color, staticBody.FinalTransform);
            }
            else
            {
                throw new InvalidOperationException($"Unknown collider component type: {staticBody.Collider.GetType()}.");
            }
        }

        foreach (var kinematicBody in _physicsState.GetKinematicBodies())
        {
            var color = GetColor(kinematicBody.Collider.IsColliding);

            if (kinematicBody.IsCircleCollider)
            {
                _debugRenderer.DrawCircle(kinematicBody.TransformedCircle, color);
            }
            else if (kinematicBody.IsRectangleCollider)
            {
                var rectangle = new AxisAlignedRectangle(((RectangleColliderComponent)kinematicBody.Collider).Dimensions);
                _debugRenderer.DrawRectangle(rectangle, color, kinematicBody.FinalTransform);
            }
            else
            {
                throw new InvalidOperationException($"Unknown collider component type: {kinematicBody.Collider.GetType()}.");
            }

            foreach (var contact in kinematicBody.Contacts)
            {
                _debugRenderer.DrawCircle(new Circle(contact.Point.WorldPosition, 3), Color.FromArgb(255, 255, 165, 0));
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

    private static Color GetColor(bool isColliding) => isColliding ? Color.Red : Color.Green;
}