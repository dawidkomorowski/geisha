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
        private readonly PhysicsState _physicsState = new PhysicsState();
        private readonly CollisionDetection _collisionDetection = new CollisionDetection();

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

            _collisionDetection.DetectCollision(physicsBodies);
        }

        public void PreparePhysicsDebugInformation()
        {
            if (_physicsConfiguration.RenderCollisionGeometry == false) return;

            foreach (var physicsBody in _physicsState.GetPhysicsBodies())
            {
                switch (physicsBody.Collider)
                {
                    case CircleColliderComponent circleColliderComponent:
                        DrawCircle(circleColliderComponent, physicsBody.FinalTransform);
                        break;
                    case RectangleColliderComponent rectangleColliderComponent:
                        DrawRectangle(rectangleColliderComponent, physicsBody.FinalTransform);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(physicsBody.Collider));
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

        private void DrawCircle(CircleColliderComponent circleColliderComponent, in Matrix3x3 transform)
        {
            var circle = new Circle(circleColliderComponent.Radius).Transform(transform);
            var color = GetColor(circleColliderComponent.IsColliding);
            _debugRenderer.DrawCircle(circle, color);
        }

        private void DrawRectangle(RectangleColliderComponent rectangleColliderComponent, in Matrix3x3 transform)
        {
            var rectangle = new AxisAlignedRectangle(rectangleColliderComponent.Dimension);
            var color = GetColor(rectangleColliderComponent.IsColliding);
            _debugRenderer.DrawRectangle(rectangle, color, transform);
        }

        private static Color GetColor(bool isColliding) =>
            isColliding ? Color.FromArgb(255, 255, 0, 0) : Color.FromArgb(255, 0, 255, 0);
    }
}