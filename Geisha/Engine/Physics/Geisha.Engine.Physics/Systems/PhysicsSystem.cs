﻿using System.Linq;
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
    internal class PhysicsSystem : IFixedTimeStepSystem
    {
        private Collider2DComponent[] _colliders = new Collider2DComponent[0];
        private Matrix3[] _transforms = new Matrix3[0];

        public string Name => GetType().FullName;

        public void FixedUpdate(Scene scene)
        {
            var entities = scene.AllEntities.Where(e => e.HasComponent<TransformComponent>() && e.HasComponent<Collider2DComponent>()).ToArray();

            if (_colliders.Length < entities.Length)
            {
                _transforms = new Matrix3[entities.Length];
                _colliders = new Collider2DComponent[entities.Length];
            }

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var collider2D = entity.GetComponent<Collider2DComponent>();

                _transforms[i] = entities[i].GetComponent<TransformComponent>().Create2DTransformationMatrix();
                _colliders[i] = collider2D;

                collider2D.ClearCollidingEntities();
            }

            for (var i = 0; i < entities.Length; i++)
            {
                var entity1 = entities[i];
                var collider1 = _colliders[i];
                var transform1 = _transforms[i];

                IShape shape1 = null;
                switch (collider1)
                {
                    case CircleColliderComponent circleCollider1:
                        shape1 = new Circle(circleCollider1.Radius).Transform(transform1).AsShape();
                        break;
                    case RectangleCollider rectangleCollider1:
                        shape1 = new Rectangle(rectangleCollider1.Dimension).Transform(transform1).AsShape();
                        break;
                }

                for (var j = i + 1; j < entities.Length; j++)
                {
                    var entity2 = entities[j];
                    var collider2 = _colliders[j];
                    var transform2 = _transforms[j];

                    IShape shape2 = null;
                    switch (collider2)
                    {
                        case CircleColliderComponent circleCollider2:
                            shape2 = new Circle(circleCollider2.Radius).Transform(transform2).AsShape();
                            break;
                        case RectangleCollider rectangleCollider2:
                            shape2 = new Rectangle(rectangleCollider2.Dimension).Transform(transform2).AsShape();
                            break;
                    }

                    if (shape1.Overlaps(shape2))
                    {
                        collider1.AddCollidingEntity(entity2);
                        collider2.AddCollidingEntity(entity1);
                    }
                }
            }
        }
    }
}