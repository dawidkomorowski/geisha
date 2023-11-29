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
    // TODO AABB optimization?
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
            var physicsBodies = _physicsState.GetPhysicsBodies();

            foreach (var physicsBody in physicsBodies)
            {
                physicsBody.UpdateFinalTransform();
            }

            _collisionDetection.DetectCollisions(physicsBodies);
        }

        public void PreparePhysicsDebugInformation()
        {
            if (!_physicsConfiguration.RenderCollisionGeometry) return;

            foreach (var physicsBody in _physicsState.GetPhysicsBodies())
            {
                if (physicsBody.IsCircleCollider)
                {
                    DrawCircle(physicsBody);
                }
                else if (physicsBody.IsRectangleCollider)
                {
                    DrawRectangle(physicsBody);
                }
                else
                {
                    throw new InvalidOperationException($"Unknown collider component type: {physicsBody.Collider.GetType()}.");
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
            }
        }

        #endregion

        private void DrawCircle(PhysicsBody physicsBody)
        {
            var color = GetColor(physicsBody.Collider.IsColliding);
            _debugRenderer.DrawCircle(physicsBody.TransformedCircle, color);
        }

        private void DrawRectangle(PhysicsBody physicsBody)
        {
            var rectangle = new AxisAlignedRectangle(((RectangleColliderComponent)physicsBody.Collider).Dimensions);
            var color = GetColor(physicsBody.Collider.IsColliding);
            _debugRenderer.DrawRectangle(rectangle, color, physicsBody.FinalTransform);
        }

        private static Color GetColor(bool isColliding) => isColliding ? Color.Red : Color.Green;
    }
}