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
    // TODO Collision Mask/Filter?
    // TODO Static objects optimization?
    // TODO Quad Tree optimization?
    // TODO Minimum Translation Vector?
    // TODO AABB optimization?
    internal sealed class PhysicsSystem : IPhysicsSystem
    {
        private readonly IDebugRenderer _debugRenderer;
        private readonly List<Collider2DComponent> _colliders = new List<Collider2DComponent>();
        private readonly List<Matrix3x3> _transforms = new List<Matrix3x3>();

        public PhysicsSystem(IDebugRenderer debugRenderer)
        {
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
            for (var i = 0; i < _colliders.Count; i++)
            {
                var collider = _colliders[i];
                var transform = _transforms[i];

                switch (collider)
                {
                    case CircleColliderComponent circleColliderComponent:
                        var circle = new Circle(circleColliderComponent.Radius).Transform(transform);
                        var color = Color.FromArgb(255, 0, 255, 0);
                        _debugRenderer.DrawCircle(circle, color);
                        break;
                    case RectangleColliderComponent rectangleColliderComponent:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(collider));
                }
            }
        }

        private static IShape CreateShapeForCollider(Collider2DComponent collider2DComponent, Matrix3x3 transform)
        {
            switch (collider2DComponent)
            {
                case CircleColliderComponent circleCollider1:
                    return new Circle(circleCollider1.Radius).Transform(transform).AsShape();
                case RectangleColliderComponent rectangleCollider1:
                    return new Rectangle(rectangleCollider1.Dimension).Transform(transform).AsShape();
                default:
                    throw new InvalidOperationException($"Unknown collider component type: {collider2DComponent.GetType()}.");
            }
        }
    }
}