using System.Linq;
using Geisha.Common.Math;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.Systems;
using NUnit.Framework;

namespace Geisha.Engine.Physics.UnitTests.Systems
{
    [TestFixture]
    public class PhysicsSystemTests
    {
        private PhysicsSystem _physicsSystem;

        [SetUp]
        public void SetUp()
        {
            _physicsSystem = new PhysicsSystem();
        }

        [Test]
        public void FixedUpdate_ShouldLeaveEntitiesNotColliding_WhenTheyWereNotCollidingAndTheyStillNotCollide()
        {
            // Arrange
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            var rectangle1 = physicsSceneBuilder.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsSceneBuilder.AddRectangleCollider(20, 0, 10, 5);
            var scene = physicsSceneBuilder.Build();

            // Assume
            Assume.That(rectangle1.GetComponent<RectangleCollider>().IsColliding, Is.False);
            Assume.That(rectangle2.GetComponent<RectangleCollider>().IsColliding, Is.False);

            // Act
            _physicsSystem.FixedUpdate(scene);

            // Assert
            Assert.That(rectangle1.GetComponent<RectangleCollider>().IsColliding, Is.False);
            Assert.That(rectangle2.GetComponent<RectangleCollider>().IsColliding, Is.False);
        }

        [Test]
        public void FixedUpdate_ShouldMakeEntitiesNotColliding_WhenTheyWereCollidingButTheyNotCollideAnymore()
        {
            // Arrange
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            var rectangle1 = physicsSceneBuilder.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsSceneBuilder.AddRectangleCollider(20, 0, 10, 5);
            var scene = physicsSceneBuilder.Build();

            rectangle1.GetComponent<RectangleCollider>().AddCollidingEntity(rectangle2);
            rectangle2.GetComponent<RectangleCollider>().AddCollidingEntity(rectangle1);

            // Assume
            Assume.That(rectangle1.GetComponent<RectangleCollider>().IsColliding, Is.True);
            Assume.That(rectangle2.GetComponent<RectangleCollider>().IsColliding, Is.True);

            // Act
            _physicsSystem.FixedUpdate(scene);

            // Assert
            Assert.That(rectangle1.GetComponent<RectangleCollider>().IsColliding, Is.False);
            Assert.That(rectangle2.GetComponent<RectangleCollider>().IsColliding, Is.False);
        }

        [Test]
        public void FixedUpdate_ShouldMakeEntitiesColliding_WhenTheyWereNotCollidingButTheyCollideNow()
        {
            // Arrange
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            var rectangle1 = physicsSceneBuilder.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsSceneBuilder.AddRectangleCollider(5, 0, 10, 5);
            var scene = physicsSceneBuilder.Build();

            // Assume
            Assume.That(rectangle1.GetComponent<RectangleCollider>().IsColliding, Is.False);
            Assume.That(rectangle2.GetComponent<RectangleCollider>().IsColliding, Is.False);

            // Act
            _physicsSystem.FixedUpdate(scene);

            // Assert
            var rectangleCollider1 = rectangle1.GetComponent<RectangleCollider>();
            Assert.That(rectangleCollider1.IsColliding, Is.True);
            Assert.That(rectangleCollider1.CollidingEntities, Has.Count.EqualTo(1));
            Assert.That(rectangleCollider1.CollidingEntities.Single(), Is.EqualTo(rectangle2));

            var rectangleCollider2 = rectangle2.GetComponent<RectangleCollider>();
            Assert.That(rectangleCollider2.IsColliding, Is.True);
            Assert.That(rectangleCollider2.CollidingEntities, Has.Count.EqualTo(1));
            Assert.That(rectangleCollider2.CollidingEntities.Single(), Is.EqualTo(rectangle1));
        }

        [Test]
        public void FixedUpdate_ShouldMakeCircleEntitiesColliding_WhenTheyWereNotCollidingButTheyCollideNow()
        {
            // Arrange
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            var circle1 = physicsSceneBuilder.AddCircleCollider(0, 0, 10);
            var circle2 = physicsSceneBuilder.AddCircleCollider(5, 0, 10);
            var scene = physicsSceneBuilder.Build();

            // Assume
            Assume.That(circle1.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
            Assume.That(circle2.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

            // Act
            _physicsSystem.FixedUpdate(scene);

            // Assert
            var circleCollider1 = circle1.GetComponent<CircleColliderComponent>();
            Assert.That(circleCollider1.IsColliding, Is.True);
            Assert.That(circleCollider1.CollidingEntities, Has.Count.EqualTo(1));
            Assert.That(circleCollider1.CollidingEntities.Single(), Is.EqualTo(circle2));

            var circleCollider2 = circle2.GetComponent<CircleColliderComponent>();
            Assert.That(circleCollider2.IsColliding, Is.True);
            Assert.That(circleCollider2.CollidingEntities, Has.Count.EqualTo(1));
            Assert.That(circleCollider2.CollidingEntities.Single(), Is.EqualTo(circle1));
        }

        [Test]
        public void FixedUpdate_ShouldMakeEntitiesCollidingAndNotCollidingWithOtherEntities_WhenThereAreManyCirclesAndRectangles()
        {
            // Arrange
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            var circle1 = physicsSceneBuilder.AddCircleCollider(0, 0, 10);
            var circle2 = physicsSceneBuilder.AddCircleCollider(15, 0, 10);
            var circle3 = physicsSceneBuilder.AddCircleCollider(50, 50, 10);
            var rectangle1 = physicsSceneBuilder.AddRectangleCollider(0, 0, 20, 10);
            var rectangle2 = physicsSceneBuilder.AddRectangleCollider(45, 45, 10, 5);
            var rectangle3 = physicsSceneBuilder.AddRectangleCollider(150, 100, 10, 5);
            var scene = physicsSceneBuilder.Build();

            // Assume
            Assume.That(circle1.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
            Assume.That(circle2.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
            Assume.That(circle3.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
            Assume.That(rectangle1.GetComponent<RectangleCollider>().IsColliding, Is.False);
            Assume.That(rectangle2.GetComponent<RectangleCollider>().IsColliding, Is.False);
            Assume.That(rectangle3.GetComponent<RectangleCollider>().IsColliding, Is.False);

            // Act
            _physicsSystem.FixedUpdate(scene);

            // Assert
            var circleCollider1 = circle1.GetComponent<CircleColliderComponent>();
            Assert.That(circleCollider1.IsColliding, Is.True);
            Assert.That(circleCollider1.CollidingEntities, Has.Count.EqualTo(2));
            Assert.That(circleCollider1.CollidingEntities, Contains.Item(circle2));
            Assert.That(circleCollider1.CollidingEntities, Contains.Item(rectangle1));

            var circleCollider2 = circle2.GetComponent<CircleColliderComponent>();
            Assert.That(circleCollider2.IsColliding, Is.True);
            Assert.That(circleCollider2.CollidingEntities, Has.Count.EqualTo(2));
            Assert.That(circleCollider2.CollidingEntities, Contains.Item(circle1));
            Assert.That(circleCollider2.CollidingEntities, Contains.Item(rectangle1));

            var circleCollider3 = circle3.GetComponent<CircleColliderComponent>();
            Assert.That(circleCollider3.IsColliding, Is.True);
            Assert.That(circleCollider3.CollidingEntities, Has.Count.EqualTo(1));
            Assert.That(circleCollider3.CollidingEntities.Single(), Is.EqualTo(rectangle2));

            var rectangleCollider1 = rectangle1.GetComponent<RectangleCollider>();
            Assert.That(rectangleCollider1.IsColliding, Is.True);
            Assert.That(rectangleCollider1.CollidingEntities, Has.Count.EqualTo(2));
            Assert.That(rectangleCollider1.CollidingEntities, Contains.Item(circle1));
            Assert.That(rectangleCollider1.CollidingEntities, Contains.Item(circle2));

            var rectangleCollider2 = rectangle2.GetComponent<RectangleCollider>();
            Assert.That(rectangleCollider2.IsColliding, Is.True);
            Assert.That(rectangleCollider2.CollidingEntities, Has.Count.EqualTo(1));
            Assert.That(rectangleCollider2.CollidingEntities, Contains.Item(circle3));

            var rectangleCollider3 = rectangle3.GetComponent<RectangleCollider>();
            Assert.That(rectangleCollider3.IsColliding, Is.False);
            Assert.That(rectangleCollider3.CollidingEntities, Has.Count.EqualTo(0));
        }

        private class PhysicsSceneBuilder
        {
            private readonly Scene _scene = new Scene();

            public Entity AddRectangleCollider(double entityX, double entityY, double rectangleWidth, double rectangleHeight)
            {
                var entity = new Entity();
                entity.AddComponent(new TransformComponent
                {
                    Translation = new Vector3(entityX, entityY, 0),
                    Rotation = Vector3.Zero,
                    Scale = Vector3.One
                });
                entity.AddComponent(new RectangleCollider {Dimension = new Vector2(rectangleWidth, rectangleHeight)});

                _scene.AddEntity(entity);

                return entity;
            }

            public Entity AddCircleCollider(double entityX, double entityY, double radius)
            {
                var entity = new Entity();
                entity.AddComponent(new TransformComponent
                {
                    Translation = new Vector3(entityX, entityY, 0),
                    Rotation = Vector3.Zero,
                    Scale = Vector3.One
                });
                entity.AddComponent(new CircleColliderComponent {Radius = radius});

                _scene.AddEntity(entity);

                return entity;
            }

            public Scene Build()
            {
                return _scene;
            }
        }
    }
}