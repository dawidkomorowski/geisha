using System.Linq;
using Geisha.Common.Math;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.Systems;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems
{
    [TestFixture]
    public class PhysicsSystemTests
    {
        private PhysicsSystem _physicsSystem = null!;

        [SetUp]
        public void SetUp()
        {
            _physicsSystem = new PhysicsSystem();
        }

        [Test]
        public void ProcessPhysics_ShouldLeaveEntitiesNotColliding_WhenTheyWereNotCollidingAndTheyStillNotCollide()
        {
            // Arrange
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            var rectangle1 = physicsSceneBuilder.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsSceneBuilder.AddRectangleCollider(20, 0, 10, 5);
            var scene = physicsSceneBuilder.Build();

            // Assume
            Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

            // Act
            _physicsSystem.ProcessPhysics(scene);

            // Assert
            Assert.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assert.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        }

        [Test]
        public void ProcessPhysics_ShouldMakeEntitiesNotColliding_WhenTheyWereCollidingButTheyNotCollideAnymore()
        {
            // Arrange
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            var rectangle1 = physicsSceneBuilder.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsSceneBuilder.AddRectangleCollider(20, 0, 10, 5);
            var scene = physicsSceneBuilder.Build();

            rectangle1.GetComponent<RectangleColliderComponent>().AddCollidingEntity(rectangle2);
            rectangle2.GetComponent<RectangleColliderComponent>().AddCollidingEntity(rectangle1);

            // Assume
            Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
            Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);

            // Act
            _physicsSystem.ProcessPhysics(scene);

            // Assert
            Assert.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assert.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        }

        [Test]
        public void ProcessPhysics_ShouldMakeEntitiesColliding_WhenTheyWereNotCollidingButTheyCollideNow()
        {
            // Arrange
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            var rectangle1 = physicsSceneBuilder.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsSceneBuilder.AddRectangleCollider(5, 0, 10, 5);
            var scene = physicsSceneBuilder.Build();

            // Assume
            Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

            // Act
            _physicsSystem.ProcessPhysics(scene);

            // Assert
            var rectangleCollider1 = rectangle1.GetComponent<RectangleColliderComponent>();
            Assert.That(rectangleCollider1.IsColliding, Is.True);
            Assert.That(rectangleCollider1.CollidingEntities, Has.Count.EqualTo(1));
            Assert.That(rectangleCollider1.CollidingEntities.Single(), Is.EqualTo(rectangle2));

            var rectangleCollider2 = rectangle2.GetComponent<RectangleColliderComponent>();
            Assert.That(rectangleCollider2.IsColliding, Is.True);
            Assert.That(rectangleCollider2.CollidingEntities, Has.Count.EqualTo(1));
            Assert.That(rectangleCollider2.CollidingEntities.Single(), Is.EqualTo(rectangle1));
        }

        [Test]
        public void ProcessPhysics_ShouldMakeCircleEntitiesColliding_WhenTheyWereNotCollidingButTheyCollideNow()
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
            _physicsSystem.ProcessPhysics(scene);

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
        public void ProcessPhysics_ShouldMakeEntitiesCollidingAndNotCollidingWithOtherEntities_WhenThereAreManyCirclesAndRectangles()
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
            Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assume.That(rectangle3.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

            // Act
            _physicsSystem.ProcessPhysics(scene);

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

            var rectangleCollider1 = rectangle1.GetComponent<RectangleColliderComponent>();
            Assert.That(rectangleCollider1.IsColliding, Is.True);
            Assert.That(rectangleCollider1.CollidingEntities, Has.Count.EqualTo(2));
            Assert.That(rectangleCollider1.CollidingEntities, Contains.Item(circle1));
            Assert.That(rectangleCollider1.CollidingEntities, Contains.Item(circle2));

            var rectangleCollider2 = rectangle2.GetComponent<RectangleColliderComponent>();
            Assert.That(rectangleCollider2.IsColliding, Is.True);
            Assert.That(rectangleCollider2.CollidingEntities, Has.Count.EqualTo(1));
            Assert.That(rectangleCollider2.CollidingEntities, Contains.Item(circle3));

            var rectangleCollider3 = rectangle3.GetComponent<RectangleColliderComponent>();
            Assert.That(rectangleCollider3.IsColliding, Is.False);
            Assert.That(rectangleCollider3.CollidingEntities, Has.Count.EqualTo(0));
        }

        [Test]
        public void ProcessPhysics_ShouldMakeEntityWithParentTransformColliding_WhenItWasNotCollidingButIsCollidingNowDueToParentTransform()
        {
            // Arrange
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            var rectangle1 = physicsSceneBuilder.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsSceneBuilder.AddRectangleColliderWithParentTransform(15, 10, -7.5, -7.5, 10, 5);
            var scene = physicsSceneBuilder.Build();

            // Assume
            Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

            // Act
            _physicsSystem.ProcessPhysics(scene);

            // Assert
            var rectangleCollider1 = rectangle1.GetComponent<RectangleColliderComponent>();
            Assert.That(rectangleCollider1.IsColliding, Is.True);
            Assert.That(rectangleCollider1.CollidingEntities, Has.Count.EqualTo(1));
            Assert.That(rectangleCollider1.CollidingEntities.Single(), Is.EqualTo(rectangle2));

            var rectangleCollider2 = rectangle2.GetComponent<RectangleColliderComponent>();
            Assert.That(rectangleCollider2.IsColliding, Is.True);
            Assert.That(rectangleCollider2.CollidingEntities, Has.Count.EqualTo(1));
            Assert.That(rectangleCollider2.CollidingEntities.Single(), Is.EqualTo(rectangle1));
        }

        [Test]
        public void ProcessPhysics_ShouldMakeEntityWithParentTransformNotColliding_WhenItWasCollidingButIsNotCollidingAnymoreDueToParentTransform()
        {
            // Arrange
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            var rectangle1 = physicsSceneBuilder.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsSceneBuilder.AddRectangleColliderWithParentTransform(10, 10, 5, 2.5, 10, 5);
            var scene = physicsSceneBuilder.Build();

            rectangle1.GetComponent<RectangleColliderComponent>().AddCollidingEntity(rectangle2);
            rectangle2.GetComponent<RectangleColliderComponent>().AddCollidingEntity(rectangle1);

            // Assume
            Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
            Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);

            // Act
            _physicsSystem.ProcessPhysics(scene);

            // Assert
            Assert.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assert.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        }

        private class PhysicsSceneBuilder
        {
            private readonly Scene _scene = new Scene();

            public Entity AddRectangleCollider(double entityX, double entityY, double rectangleWidth, double rectangleHeight)
            {
                var entity = CreateRectangleCollider(entityX, entityY, rectangleWidth, rectangleHeight);
                _scene.AddEntity(entity);
                return entity;
            }

            public Entity AddRectangleColliderWithParentTransform(double parentX, double parentY, double entityX, double entityY, double rectangleWidth,
                double rectangleHeight)
            {
                var parent = new Entity();
                parent.AddComponent(new TransformComponent
                {
                    Translation = new Vector3(parentX, parentY, 0),
                    Rotation = Vector3.Zero,
                    Scale = Vector3.One
                });

                var child = CreateRectangleCollider(entityX, entityY, rectangleWidth, rectangleHeight);
                parent.AddChild(child);

                _scene.AddEntity(parent);

                return child;
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

            private static Entity CreateRectangleCollider(double entityX, double entityY, double rectangleWidth, double rectangleHeight)
            {
                var entity = new Entity();
                entity.AddComponent(new TransformComponent
                {
                    Translation = new Vector3(entityX, entityY, 0),
                    Rotation = Vector3.Zero,
                    Scale = Vector3.One
                });
                entity.AddComponent(new RectangleColliderComponent {Dimension = new Vector2(rectangleWidth, rectangleHeight)});
                return entity;
            }
        }
    }
}