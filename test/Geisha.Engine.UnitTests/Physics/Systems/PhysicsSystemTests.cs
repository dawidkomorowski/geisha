using System.Linq;
using Geisha.Common.Math;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Diagnostics;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.Physics.Systems;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Systems
{
    [TestFixture]
    public class PhysicsSystemTests
    {
        private readonly Color _colorWhenNotColliding = Color.FromArgb(255, 0, 255, 0);
        private readonly Color _colorWhenColliding = Color.FromArgb(255, 255, 0, 0);
        private readonly PhysicsConfiguration.IBuilder _configurationBuilder = PhysicsConfiguration.CreateBuilder();
        private IDebugRenderer _debugRenderer = null!;

        [SetUp]
        public void SetUp()
        {
            _debugRenderer = Substitute.For<IDebugRenderer>();
        }

        [Test]
        public void ProcessPhysics_ShouldLeaveEntitiesNotColliding_WhenTheyWereNotCollidingAndTheyStillNotCollide()
        {
            // Arrange
            var physicsSystem = GetPhysicsSystem();
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            var rectangle1 = physicsSceneBuilder.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsSceneBuilder.AddRectangleCollider(20, 0, 10, 5);
            var scene = physicsSceneBuilder.Build();

            // Assume
            Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

            // Act
            physicsSystem.ProcessPhysics(scene);

            // Assert
            Assert.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assert.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        }

        [Test]
        public void ProcessPhysics_ShouldMakeEntitiesNotColliding_WhenTheyWereCollidingButTheyNotCollideAnymore()
        {
            // Arrange
            var physicsSystem = GetPhysicsSystem();
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
            physicsSystem.ProcessPhysics(scene);

            // Assert
            Assert.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assert.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        }

        [Test]
        public void ProcessPhysics_ShouldMakeEntitiesColliding_WhenTheyWereNotCollidingButTheyCollideNow()
        {
            // Arrange
            var physicsSystem = GetPhysicsSystem();
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            var rectangle1 = physicsSceneBuilder.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsSceneBuilder.AddRectangleCollider(5, 0, 10, 5);
            var scene = physicsSceneBuilder.Build();

            // Assume
            Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

            // Act
            physicsSystem.ProcessPhysics(scene);

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
            var physicsSystem = GetPhysicsSystem();
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            var circle1 = physicsSceneBuilder.AddCircleCollider(0, 0, 10);
            var circle2 = physicsSceneBuilder.AddCircleCollider(5, 0, 10);
            var scene = physicsSceneBuilder.Build();

            // Assume
            Assume.That(circle1.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
            Assume.That(circle2.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

            // Act
            physicsSystem.ProcessPhysics(scene);

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
            var physicsSystem = GetPhysicsSystem();
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
            physicsSystem.ProcessPhysics(scene);

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
            var physicsSystem = GetPhysicsSystem();
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            var rectangle1 = physicsSceneBuilder.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsSceneBuilder.AddRectangleColliderWithParentTransform(15, 10, -7.5, -7.5, 10, 5);
            var scene = physicsSceneBuilder.Build();

            // Assume
            Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

            // Act
            physicsSystem.ProcessPhysics(scene);

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
            var physicsSystem = GetPhysicsSystem();
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
            physicsSystem.ProcessPhysics(scene);

            // Assert
            Assert.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assert.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        }

        [TestCase(false, 0)]
        [TestCase(true, 1)]
        public void PreparePhysicsDebugInformation_ShouldDrawCircleForCircleCollider_WhenCollisionGeometryRenderingIsEnabled(bool renderCollisionGeometry,
            int expectedDrawCallsCount)
        {
            // Arrange
            _configurationBuilder.WithRenderCollisionGeometry(renderCollisionGeometry);
            var physicsSystem = GetPhysicsSystem();
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            physicsSceneBuilder.AddCircleCollider(10, 20, 30);
            var scene = physicsSceneBuilder.Build();

            physicsSystem.ProcessPhysics(scene);

            // Act
            physicsSystem.PreparePhysicsDebugInformation();

            // Assert
            var circle = new Circle(new Vector2(10, 20), 30);
            _debugRenderer.Received(expectedDrawCallsCount).DrawCircle(circle, _colorWhenNotColliding);
        }

        [TestCase(false, 0)]
        [TestCase(true, 1)]
        public void PreparePhysicsDebugInformation_ShouldDrawRectangleForRectangleCollider_WhenCollisionGeometryRenderingIsEnabled(bool renderCollisionGeometry,
            int expectedDrawCallsCount)
        {
            // Arrange
            _configurationBuilder.WithRenderCollisionGeometry(renderCollisionGeometry);
            var physicsSystem = GetPhysicsSystem();
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            var entity = physicsSceneBuilder.AddRectangleCollider(10, 20, 100, 200);
            var scene = physicsSceneBuilder.Build();

            physicsSystem.ProcessPhysics(scene);

            // Act
            physicsSystem.PreparePhysicsDebugInformation();

            // Assert
            var rectangle = new AxisAlignedRectangle(new Vector2(100, 200));
            var transform = entity.GetComponent<Transform2DComponent>().ToMatrix();
            _debugRenderer.Received(expectedDrawCallsCount).DrawRectangle(rectangle, _colorWhenNotColliding, transform);
        }

        [Test]
        public void PreparePhysicsDebugInformation_ShouldDrawCollisionGeometryWithDifferentColor_WhenEntityIsColliding()
        {
            // Arrange
            _configurationBuilder.WithRenderCollisionGeometry(true);
            var physicsSystem = GetPhysicsSystem();
            var physicsSceneBuilder = new PhysicsSceneBuilder();
            var circleEntity = physicsSceneBuilder.AddCircleCollider(10, 20, 30);
            var rectangleEntity = physicsSceneBuilder.AddRectangleCollider(10, 20, 100, 200);
            var scene = physicsSceneBuilder.Build();

            physicsSystem.ProcessPhysics(scene);

            // Assume
            Assume.That(circleEntity.GetComponent<CircleColliderComponent>().IsColliding, Is.True);
            Assume.That(rectangleEntity.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);

            // Act
            physicsSystem.PreparePhysicsDebugInformation();

            // Assert
            var circle = new Circle(new Vector2(10, 20), 30);
            _debugRenderer.Received(1).DrawCircle(circle, _colorWhenColliding);

            var rectangle = new AxisAlignedRectangle(new Vector2(100, 200));
            var transform = rectangleEntity.GetComponent<Transform2DComponent>().ToMatrix();
            _debugRenderer.Received(1).DrawRectangle(rectangle, _colorWhenColliding, transform);
        }

        private PhysicsSystem GetPhysicsSystem()
        {
            return new PhysicsSystem(_configurationBuilder.Build(), _debugRenderer);
        }

        private class PhysicsSceneBuilder
        {
            private readonly Scene _scene = TestSceneFactory.Create();

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
                parent.AddComponent(new Transform2DComponent
                {
                    Translation = new Vector2(parentX, parentY),
                    Rotation = 0,
                    Scale = Vector2.One
                });

                var child = CreateRectangleCollider(entityX, entityY, rectangleWidth, rectangleHeight);
                parent.AddChild(child);

                _scene.AddEntity(parent);

                return child;
            }

            public Entity AddCircleCollider(double entityX, double entityY, double radius)
            {
                var entity = new Entity();
                entity.AddComponent(new Transform2DComponent
                {
                    Translation = new Vector2(entityX, entityY),
                    Rotation = 0,
                    Scale = Vector2.One
                });
                entity.AddComponent(new CircleColliderComponent { Radius = radius });

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
                entity.AddComponent(new Transform2DComponent
                {
                    Translation = new Vector2(entityX, entityY),
                    Rotation = 0,
                    Scale = Vector2.One
                });
                entity.AddComponent(new RectangleColliderComponent { Dimension = new Vector2(rectangleWidth, rectangleHeight) });
                return entity;
            }
        }
    }
}