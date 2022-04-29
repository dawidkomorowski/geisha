using System;
using System.Collections.Generic;
using Geisha.Common.Math;
using Geisha.Common.Math.SAT;
using Geisha.Engine.Physics.Components;

namespace Geisha.Engine.Physics.Systems
{
    internal sealed class CollisionDetection
    {
        public void DetectCollision(IReadOnlyList<PhysicsBody> physicsBodies)
        {
            foreach (var physicsBody in physicsBodies)
            {
                physicsBody.Collider.ClearCollidingEntities();
            }

            for (var i = 0; i < physicsBodies.Count; i++)
            {
                var physicsBody1 = physicsBodies[i];

                var shape1 = CreateShapeForCollider(physicsBody1.Collider, physicsBody1.FinalTransform);

                for (var j = i + 1; j < physicsBodies.Count; j++)
                {
                    var physicsBody2 = physicsBodies[j];

                    var shape2 = CreateShapeForCollider(physicsBody2.Collider, physicsBody2.FinalTransform);

                    if (shape1.Overlaps(shape2))
                    {
                        physicsBody1.Collider.AddCollidingEntity(physicsBody2.Entity);
                        physicsBody2.Collider.AddCollidingEntity(physicsBody1.Entity);
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