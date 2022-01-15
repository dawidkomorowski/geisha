using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Common.Math;
using Geisha.Common.Math.SAT;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Physics.Components;

namespace Geisha.Engine.Physics.Systems
{
    // TODO Collision Mask/Filter/Group?
    // TODO Static objects optimization?
    // TODO Quad Tree optimization / Broad Phase?
    // TODO Minimum Translation Vector?
    // TODO AABB optimization?
    internal sealed class PhysicsSystem : IPhysicsSystem
    {
        private readonly PhysicsConfiguration _physicsConfiguration;
        private readonly IDebugRenderer _debugRenderer;
        private readonly List<Collider2DComponent> _colliders = new List<Collider2DComponent>();
        private readonly List<Matrix3x3> _transforms = new List<Matrix3x3>();

        public PhysicsSystem(PhysicsConfiguration physicsConfiguration, IDebugRenderer debugRenderer)
        {
            _physicsConfiguration = physicsConfiguration;
            _debugRenderer = debugRenderer;
        }

        public void ProcessPhysics(Scene scene)
        {
            var entities = scene.AllEntities.Where(e => e.HasComponent<Transform2DComponent>() && e.HasComponent<Collider2DComponent>()).ToArray();

            _colliders.Clear();
            _transforms.Clear();

            foreach (var entity in entities)
            {
                var collider2D = entity.GetComponent<Collider2DComponent>();

                _transforms.Add(TransformHierarchy.Calculate2DTransformationMatrix(entity));
                _colliders.Add(collider2D);

                collider2D.ClearCollidingEntities();
            }

            for (var i = 0; i < entities.Length; i++)
            {
                var entity1 = entities[i];
                var collider1 = _colliders[i];
                var transform1 = _transforms[i];

                var shape1 = CreateShapeForCollider(collider1, transform1);

                for (var j = i + 1; j < entities.Length; j++)
                {
                    var entity2 = entities[j];
                    var collider2 = _colliders[j];
                    var transform2 = _transforms[j];

                    IShape shape2 = CreateShapeForCollider(collider2, transform2);

                    if (shape1.Overlaps(shape2))
                    {
                        collider1.AddCollidingEntity(entity2);
                        collider2.AddCollidingEntity(entity1);
                    }
                }
            }
        }

        public void PreparePhysicsDebugInformation()
        {
            if (_physicsConfiguration.RenderCollisionGeometry == false) return;

            for (var i = 0; i < _colliders.Count; i++)
            {
                var collider = _colliders[i];
                var transform = _transforms[i];

                switch (collider)
                {
                    case CircleColliderComponent circleColliderComponent:
                        DrawCircle(circleColliderComponent, transform);
                        break;
                    case RectangleColliderComponent rectangleColliderComponent:
                        DrawRectangle(rectangleColliderComponent, transform);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(collider));
                }
            }
        }

        private void DrawCircle(CircleColliderComponent circleColliderComponent, Matrix3x3 transform)
        {
            var circle = new Circle(circleColliderComponent.Radius).Transform(transform);
            var color = GetColor(circleColliderComponent.IsColliding);
            _debugRenderer.DrawCircle(circle, color);
        }

        private void DrawRectangle(RectangleColliderComponent rectangleColliderComponent, Matrix3x3 transform)
        {
            var rectangle = new Rectangle(rectangleColliderComponent.Dimension);
            var color = GetColor(rectangleColliderComponent.IsColliding);
            _debugRenderer.DrawRectangle(rectangle, color, transform);
        }

        private static Color GetColor(bool isColliding) =>
            isColliding ? Color.FromArgb(255, 255, 0, 0) : Color.FromArgb(255, 0, 255, 0);

        private static IShape CreateShapeForCollider(Collider2DComponent collider2DComponent, Matrix3x3 transform)
        {
            return collider2DComponent switch
            {
                CircleColliderComponent circleCollider => new Circle(circleCollider.Radius).Transform(transform).AsShape(),
                RectangleColliderComponent rectangleCollider => new Rectangle(rectangleCollider.Dimension).Transform(transform).AsShape(),
                _ => throw new InvalidOperationException($"Unknown collider component type: {collider2DComponent.GetType()}.")
            };
        }
    }
}