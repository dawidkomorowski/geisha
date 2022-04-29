using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Common.Math;
using Geisha.Common.Math.SAT;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Physics.Components;

namespace Geisha.Engine.Physics.Systems
{
    internal sealed class CollisionDetection
    {
        private readonly List<Collider2DComponent> _colliders = new List<Collider2DComponent>();
        private readonly List<Matrix3x3> _transforms = new List<Matrix3x3>();

        public void DetectCollision(IReadOnlyList<PhysicsBody> physicsBodies)
        {
            var entities = physicsBodies.Select(pb => pb.Entity).ToArray();

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