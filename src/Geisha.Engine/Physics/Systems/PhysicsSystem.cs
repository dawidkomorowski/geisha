using System;
using System.Linq;
using Geisha.Common.Math;
using Geisha.Common.Math.SAT;
using Geisha.Engine.Core.Components;
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
        private Collider2DComponent[] _colliders = new Collider2DComponent[0];
        private Matrix3x3[] _transforms = new Matrix3x3[0];

        public void ProcessPhysics(Scene scene)
        {
            var entities = scene.AllEntities.Where(e => e.HasComponent<TransformComponent>() && e.HasComponent<Collider2DComponent>()).ToArray();

            if (_colliders.Length < entities.Length)
            {
                _transforms = new Matrix3x3[entities.Length];
                _colliders = new Collider2DComponent[entities.Length];
            }

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var collider2D = entity.GetComponent<Collider2DComponent>();

                _transforms[i] = TransformHierarchy.CalculateTransformationMatrix(entities[i]);
                _colliders[i] = collider2D;

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
                CircleColliderComponent circleCollider1 => new Circle(circleCollider1.Radius).Transform(transform).AsShape(),
                RectangleColliderComponent rectangleCollider1 => new Rectangle(rectangleCollider1.Dimension).Transform(transform).AsShape(),
                _ => throw new InvalidOperationException($"Unknown collider component type: {collider2DComponent.GetType()}.")
            };
        }
    }
}