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
    // TODO Quad Tree optimization?
    // TODO Separating Axis Theorem?
    // TODO Minimum Translation Vector?
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
                var entity1 = entities[i];
                var transform1 = _transforms[i];
                var translation1 = Matrix3.Translation(transform1.Translation.ToVector2());

                switch (_colliders[i])
                {
                    case CircleCollider circleCollider1:
                    {
                        var circle1 = circleCollider1.Circle.Transform(translation1);

                        for (var j = i + 1; j < entities.Length; j++)
                        {
                            var entity2 = entities[j];
                            var transform2 = _transforms[j];
                            var translation2 = Matrix3.Translation(transform2.Translation.ToVector2());

                            switch (_colliders[j])
                            {
                                case CircleCollider circleCollider2:
                                    var circle2 = circleCollider2.Circle.Transform(translation2);

                                    if (circle1.Overlaps(circle2))
                                    {
                                        circleCollider1.AddCollidingEntity(entity2);
                                        circleCollider2.AddCollidingEntity(entity1);
                                    }
                                    break;
                                case RectangleCollider rectangleCollider2:
                                    break;
                            }
                        }
                        break;
                    }
                    case RectangleCollider rectangleCollider1:
                    {
                        var rectangle1 = rectangleCollider1.Rectangle.Transform(translation1);

                        for (var j = i + 1; j < entities.Length; j++)
                        {
                            var entity2 = entities[j];
                            var transform2 = _transforms[j];
                            var translation2 = Matrix3.Translation(transform2.Translation.ToVector2());

                            switch (_colliders[j])
                            {
                                case CircleCollider circleCollider2:
                                    break;
                                case RectangleCollider rectangleCollider2:
                                    break;
                            }
                        }
                        break;
                    }
                }
            }
        }
    }
}