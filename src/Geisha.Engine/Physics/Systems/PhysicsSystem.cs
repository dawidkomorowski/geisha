using System;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;

namespace Geisha.Engine.Physics.Systems
{
    // TODO Collision Mask/Filter/Group?
    // TODO Static objects optimization?
    // TODO Quad Tree optimization / Broad Phase?
    // TODO Minimum Translation Vector?
    //
    // TODO Proved optimizations:
    // TODO - AABB optimization
    // TODO - use for instead of foreach to avoid allocations
    internal sealed class PhysicsSystem : IPhysicsGameLoopStep, ISceneObserver
    {
        private readonly PhysicsConfiguration _physicsConfiguration;
        private readonly IDebugRenderer _debugRenderer;
        private readonly PhysicsState _physicsState = new();
        private readonly CollisionDetection _collisionDetection = new();

        public PhysicsSystem(PhysicsConfiguration physicsConfiguration, IDebugRenderer debugRenderer)
        {
            _physicsConfiguration = physicsConfiguration;
            _debugRenderer = debugRenderer;
        }

        #region Implementation of IPhysicsGameLoopStep

        public void ProcessPhysics()
        {
            var kinematicBodies = _physicsState.GetKinematicBodies();

            foreach (var kinematicBody in kinematicBodies)
            {
                kinematicBody.UpdateFinalTransform();
            }

            _collisionDetection.DetectCollisions(kinematicBodies);
        }

        public void PreparePhysicsDebugInformation()
        {
            if (!_physicsConfiguration.RenderCollisionGeometry) return;

            foreach (var kinematicBody in _physicsState.GetKinematicBodies())
            {
                if (kinematicBody.IsCircleCollider)
                {
                    DrawCircle(kinematicBody);
                }
                else if (kinematicBody.IsRectangleCollider)
                {
                    DrawRectangle(kinematicBody);
                }
                else
                {
                    throw new InvalidOperationException($"Unknown collider component type: {kinematicBody.Collider.GetType()}.");
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

        private void DrawCircle(KinematicBody kinematicBody)
        {
            var color = GetColor(kinematicBody.Collider.IsColliding);
            _debugRenderer.DrawCircle(kinematicBody.TransformedCircle, color);
        }

        private void DrawRectangle(KinematicBody kinematicBody)
        {
            var rectangle = new AxisAlignedRectangle(((RectangleColliderComponent)kinematicBody.Collider).Dimensions);
            var color = GetColor(kinematicBody.Collider.IsColliding);
            _debugRenderer.DrawRectangle(rectangle, color, kinematicBody.FinalTransform);
        }

        private static Color GetColor(bool isColliding) => isColliding ? Color.Red : Color.Green;
    }
}