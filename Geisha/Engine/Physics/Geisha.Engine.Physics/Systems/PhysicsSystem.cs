using System.ComponentModel.Composition;
using System.Linq;
using Geisha.Common.Math;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Physics.Components;

namespace Geisha.Engine.Physics.Systems
{
    // TODO Collision Mask/Filter?
    // TODO Static objects optimization?
    // TODO Separating Axis Theorem?
    [Export(typeof(IFixedTimeStepSystem))]
    public class PhysicsSystem : IFixedTimeStepSystem
    {
        private Collider2D[] _colliders = new Collider2D[0];
        private Transform[] _transforms = new Transform[0];
        public int Priority { get; set; } = 2;

        public void FixedUpdate(Scene scene)
        {
            var entities = scene.AllEntities.Where(e => e.HasComponent<Transform>() && e.HasComponent<Collider2D>()).ToArray();

            if (_colliders.Length < entities.Length)
            {
                _transforms = new Transform[entities.Length];
                _colliders = new Collider2D[entities.Length];
            }

            for (var i = 0; i < entities.Length; i++)
            {
                var entity = entities[i];
                var collider2D = entity.GetComponent<Collider2D>();

                _transforms[i] = entities[i].GetComponent<Transform>();
                _colliders[i] = collider2D;

                collider2D.ClearCollidingEntities();
            }

            for (var i = 0; i < entities.Length; i++)
            {
                // TODO Do first check here to avoid checking first body each time and getting its collision shape and transform etc

                for (var j = i + 1; j < entities.Length; j++)
                {
                    var entity1 = entities[i];
                    var entity2 = entities[j];
                    var transform1 = _transforms[i];
                    var transform2 = _transforms[j];

                    {
                        if (_colliders[i] is CircleCollider circleCollider1 && _colliders[j] is CircleCollider circleCollider2)
                        {
                            var circle1 = circleCollider1.Circle.Transform(Matrix3.Translation(transform1.Translation.ToVector2()));
                            var circle2 = circleCollider2.Circle.Transform(Matrix3.Translation(transform2.Translation.ToVector2()));

                            if (circle1.Overlaps(circle2))
                            {
                                circleCollider1.AddCollidingEntity(entity2);
                                circleCollider2.AddCollidingEntity(entity1);
                            }
                        }
                    }

                    {
                        if (_colliders[i] is RectangleCollider rectangleCollider1 && _colliders[j] is RectangleCollider rectangleCollider2)
                        {
                        }
                    }

                    {
                        if (_colliders[i] is CircleCollider circleCollider1 && _colliders[j] is RectangleCollider rectangleCollider2)
                        {
                        }
                    }

                    {
                        if (_colliders[i] is RectangleCollider rectangleCollider1 && _colliders[j] is CircleCollider circleCollider2)
                        {
                        }
                    }
                }
            }
        }
    }
}