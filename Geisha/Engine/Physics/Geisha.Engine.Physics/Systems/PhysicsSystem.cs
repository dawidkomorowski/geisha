using System.ComponentModel.Composition;
using System.Linq;
using Geisha.Common.Math;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.Engine.Physics.Components;

namespace Geisha.Engine.Physics.Systems
{
    [Export(typeof(IFixedTimeStepSystem))]
    public class PhysicsSystem : IFixedTimeStepSystem
    {
        public int Priority { get; set; } = 2;

        public void FixedUpdate(Scene scene)
        {
            var entities = scene.AllEntities.Where(e => e.HasComponent<Transform>() && e.HasComponent<Collider2D>()).ToList();

            foreach (var entity in entities)
            {
                var collider2D = entity.GetComponent<Collider2D>();
                collider2D.ClearCollidingEntities();
            }

            foreach (var entity1 in entities)
            {
                foreach (var entity2 in entities)
                {
                    var transform1 = entity1.GetComponent<Transform>();
                    var transform2 = entity2.GetComponent<Transform>();

                    if (entity1.HasComponent<CircleCollider>() && entity2.HasComponent<CircleCollider>() && entity1 != entity2)
                    {
                        var circleCollider1 = entity1.GetComponent<CircleCollider>();
                        var circleCollider2 = entity2.GetComponent<CircleCollider>();

                        var circle1 = circleCollider1.Circle.Transform(Matrix3.Translation(transform1.Translation.ToVector2()));
                        var circle2 = circleCollider2.Circle.Transform(Matrix3.Translation(transform2.Translation.ToVector2()));

                        if (circle1.Overlaps(circle2))
                        {
                            circleCollider1.AddCollidingEntity(entity2);
                            circleCollider2.AddCollidingEntity(entity1);
                        }
                    }

                    if (entity1.HasComponent<RectangleCollider>() && entity2.HasComponent<RectangleCollider>() && entity1 != entity2)
                    {
                        var rectangleCollider1 = entity1.GetComponent<RectangleCollider>();
                        var rectangleCollider2 = entity2.GetComponent<RectangleCollider>();
                    }

                    if (entity1.HasComponent<CircleCollider>() && entity2.HasComponent<RectangleCollider>() && entity1 != entity2)
                    {
                        var circleCollider = entity1.GetComponent<CircleCollider>();
                        var rectangleCollider = entity2.GetComponent<RectangleCollider>();
                    }
                }
            }
        }
    }
}