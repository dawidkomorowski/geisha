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
            var (physicsSystem, physicsScene) = GetPhysicsSystem();
            var rectangle1 = physicsScene.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsScene.AddRectangleCollider(20, 0, 10, 5);

            // Assume
            Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

            // Act
            physicsSystem.ProcessPhysics();

            // Assert
            Assert.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assert.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        }

        [Test]
        public void ProcessPhysics_ShouldMakeEntitiesNotColliding_WhenTheyWereCollidingButTheyNotCollideAnymore()
        {
            // Arrange
            var (physicsSystem, physicsScene) = GetPhysicsSystem();
            var rectangle1 = physicsScene.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsScene.AddRectangleCollider(20, 0, 10, 5);

            rectangle1.GetComponent<RectangleColliderComponent>().AddCollidingEntity(rectangle2);
            rectangle2.GetComponent<RectangleColliderComponent>().AddCollidingEntity(rectangle1);

            // Assume
            Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
            Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);

            // Act
            physicsSystem.ProcessPhysics();

            // Assert
            Assert.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assert.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        }

        [Test]
        public void ProcessPhysics_ShouldMakeEntitiesColliding_WhenTheyWereNotCollidingButTheyCollideNow()
        {
            // Arrange
            var (physicsSystem, physicsScene) = GetPhysicsSystem();
            var rectangle1 = physicsScene.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsScene.AddRectangleCollider(5, 0, 10, 5);

            // Assume
            Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

            // Act
            physicsSystem.ProcessPhysics();

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
            var (physicsSystem, physicsScene) = GetPhysicsSystem();
            var circle1 = physicsScene.AddCircleCollider(0, 0, 10);
            var circle2 = physicsScene.AddCircleCollider(5, 0, 10);

            // Assume
            Assume.That(circle1.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
            Assume.That(circle2.GetComponent<CircleColliderComponent>().IsColliding, Is.False);

            // Act
            physicsSystem.ProcessPhysics();

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
            var (physicsSystem, physicsScene) = GetPhysicsSystem();
            var circle1 = physicsScene.AddCircleCollider(0, 0, 10);
            var circle2 = physicsScene.AddCircleCollider(15, 0, 10);
            var circle3 = physicsScene.AddCircleCollider(50, 50, 10);
            var rectangle1 = physicsScene.AddRectangleCollider(0, 0, 20, 10);
            var rectangle2 = physicsScene.AddRectangleCollider(45, 45, 10, 5);
            var rectangle3 = physicsScene.AddRectangleCollider(150, 100, 10, 5);

            // Assume
            Assume.That(circle1.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
            Assume.That(circle2.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
            Assume.That(circle3.GetComponent<CircleColliderComponent>().IsColliding, Is.False);
            Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assume.That(rectangle3.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

            // Act
            physicsSystem.ProcessPhysics();

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
            var (physicsSystem, physicsScene) = GetPhysicsSystem();
            var rectangle1 = physicsScene.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsScene.AddRectangleColliderWithParentTransform(15, 10, -7.5, -7.5, 10, 5);

            // Assume
            Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);

            // Act
            physicsSystem.ProcessPhysics();

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
            var (physicsSystem, physicsScene) = GetPhysicsSystem();
            var rectangle1 = physicsScene.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsScene.AddRectangleColliderWithParentTransform(10, 10, 5, 2.5, 10, 5);

            rectangle1.GetComponent<RectangleColliderComponent>().AddCollidingEntity(rectangle2);
            rectangle2.GetComponent<RectangleColliderComponent>().AddCollidingEntity(rectangle1);

            // Assume
            Assume.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);
            Assume.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.True);

            // Act
            physicsSystem.ProcessPhysics();

            // Assert
            Assert.That(rectangle1.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
            Assert.That(rectangle2.GetComponent<RectangleColliderComponent>().IsColliding, Is.False);
        }

        [Test]
        public void ProcessPhysics_ShouldNotMakeEntitiesColliding_WhenTransform2DComponentRemoved()
        {
            // Arrange
            var (physicsSystem, physicsScene) = GetPhysicsSystem();
            var rectangle1 = physicsScene.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsScene.AddRectangleCollider(5, 0, 10, 5);

            var rectangleCollider1 = rectangle1.GetComponent<RectangleColliderComponent>();
            var rectangleCollider2 = rectangle2.GetComponent<RectangleColliderComponent>();

            rectangle1.RemoveComponent(rectangle1.GetComponent<Transform2DComponent>());

            // Assume
            Assume.That(rectangleCollider1.IsColliding, Is.False);
            Assume.That(rectangleCollider2.IsColliding, Is.False);

            // Act
            physicsSystem.ProcessPhysics();

            // Assert
            Assert.That(rectangleCollider1.IsColliding, Is.False);
            Assert.That(rectangleCollider1.CollidingEntities, Has.Count.Zero);

            Assert.That(rectangleCollider2.IsColliding, Is.False);
            Assert.That(rectangleCollider2.CollidingEntities, Has.Count.Zero);
        }

        [Test]
        public void ProcessPhysics_ShouldNotMakeEntitiesColliding_WhenCollider2DComponentRemoved()
        {
            // Arrange
            var (physicsSystem, physicsScene) = GetPhysicsSystem();
            var rectangle1 = physicsScene.AddRectangleCollider(0, 0, 10, 5);
            var rectangle2 = physicsScene.AddRectangleCollider(5, 0, 10, 5);

            var rectangleCollider1 = rectangle1.GetComponent<RectangleColliderComponent>();
            var rectangleCollider2 = rectangle2.GetComponent<RectangleColliderComponent>();

            rectangle1.RemoveComponent(rectangleCollider1);

            // Assume
            Assume.That(rectangleCollider1.IsColliding, Is.False);
            Assume.That(rectangleCollider2.IsColliding, Is.False);

            // Act
            physicsSystem.ProcessPhysics();

            // Assert
            Assert.That(rectangleCollider1.IsColliding, Is.False);
            Assert.That(rectangleCollider1.CollidingEntities, Has.Count.Zero);

            Assert.That(rectangleCollider2.IsColliding, Is.False);
            Assert.That(rectangleCollider2.CollidingEntities, Has.Count.Zero);
        }

        [TestCase(false, 0)]
        [TestCase(true, 1)]
        public void PreparePhysicsDebugInformation_ShouldDrawCircleForCircleCollider_WhenCollisionGeometryRenderingIsEnabled(bool renderCollisionGeometry,
            int expectedDrawCallsCount)
        {
            // Arrange
            _configurationBuilder.WithRenderCollisionGeometry(renderCollisionGeometry);
            var (physicsSystem, physicsScene) = GetPhysicsSystem();
            physicsScene.AddCircleCollider(10, 20, 30);

            physicsSystem.ProcessPhysics();

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
            var (physicsSystem, physicsScene) = GetPhysicsSystem();
            var entity = physicsScene.AddRectangleCollider(10, 20, 100, 200);

            physicsSystem.ProcessPhysics();

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
            var (physicsSystem, physicsScene) = GetPhysicsSystem();
            var circleEntity = physicsScene.AddCircleCollider(10, 20, 30);
            var rectangleEntity = physicsScene.AddRectangleCollider(10, 20, 100, 200);

            physicsSystem.ProcessPhysics();

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

        private (PhysicsSystem physicsSystem, PhysicsScene physicsScene) GetPhysicsSystem()
        {
            var physicsSystem = new PhysicsSystem(_configurationBuilder.Build(), _debugRenderer);
            var physicsScene = new PhysicsScene(physicsSystem);
            return (physicsSystem, physicsScene);
        }

        private class PhysicsScene
        {
            private readonly Scene _scene = TestSceneFactory.Create();

            public PhysicsScene(ISceneObserver observer)
            {
                _scene.AddObserver(observer);
            }

            public Entity AddRectangleCollider(double entityX, double entityY, double rectangleWidth, double rectangleHeight)
            {
                var entity = _scene.CreateEntity();
                CreateRectangleCollider(entity, entityX, entityY, rectangleWidth, rectangleHeight);
                return entity;
            }

            public Entity AddRectangleColliderWithParentTransform(double parentX, double parentY, double entityX, double entityY, double rectangleWidth,
                double rectangleHeight)
            {
                var parent = _scene.CreateEntity();

                var transform2DComponent = parent.CreateComponent<Transform2DComponent>();
                transform2DComponent.Translation = new Vector2(parentX, parentY);
                transform2DComponent.Rotation = 0;
                transform2DComponent.Scale = Vector2.One;

                var child = parent.CreateChildEntity();
                CreateRectangleCollider(child, entityX, entityY, rectangleWidth, rectangleHeight);

                return child;
            }

            public Entity AddCircleCollider(double entityX, double entityY, double radius)
            {
                var entity = _scene.CreateEntity();

                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                transform2DComponent.Translation = new Vector2(entityX, entityY);
                transform2DComponent.Rotation = 0;
                transform2DComponent.Scale = Vector2.One;

                var circleColliderComponent = entity.CreateComponent<CircleColliderComponent>();
                circleColliderComponent.Radius = radius;

                return entity;
            }

            private static void CreateRectangleCollider(Entity entity, double entityX, double entityY, double rectangleWidth, double rectangleHeight)
            {
                var transform2DComponent = entity.CreateComponent<Transform2DComponent>();
                transform2DComponent.Translation = new Vector2(entityX, entityY);
                transform2DComponent.Rotation = 0;
                transform2DComponent.Scale = Vector2.One;

                var rectangleColliderComponent = entity.CreateComponent<RectangleColliderComponent>();
                rectangleColliderComponent.Dimension = new Vector2(rectangleWidth, rectangleHeight);
            }
        }
    }
}