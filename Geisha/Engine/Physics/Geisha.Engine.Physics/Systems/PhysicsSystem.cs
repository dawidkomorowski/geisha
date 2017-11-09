using System.ComponentModel.Composition;
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
                var collider1 = _colliders[i];
                var transformationMatrix1 = transform1.Create2DTransformationMatrix();

                IShape shape1 = null;
                switch (collider1)
                {
                    case CircleCollider circleCollider1:
                    {
                        shape1 = new Circle(circleCollider1.Radius).Transform(transformationMatrix1).AsShape();
                        break;
                    }
                    case RectangleCollider rectangleCollider1:
                    {
                        shape1 = new Rectangle(rectangleCollider1.Dimension).Transform(transformationMatrix1).AsShape();
                        break;
                    }
                }

                for (var j = i + 1; j < entities.Length; j++)
                {
                    var entity2 = entities[j];
                    var transform2 = _transforms[j];
                    var collider2 = _colliders[j];
                    var transformationMatrix2 = transform2.Create2DTransformationMatrix();

                    IShape shape2 = null;
                    switch (collider2)
                    {
                        case CircleCollider circleCollider2:
                            shape2 = new Circle(circleCollider2.Radius).Transform(transformationMatrix2).AsShape();
                            break;
                        case RectangleCollider rectangleCollider2:
                            shape2 = new Rectangle(rectangleCollider2.Dimension).Transform(transformationMatrix2).AsShape();
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