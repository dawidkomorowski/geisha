using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Components;

[TestFixture]
public class Collider2DComponentTests
{
    private Scene Scene { get; set; } = null!;
    private Entity Entity { get; set; } = null!;

    [SetUp]
    public void SetUp()
    {
        Scene = TestSceneFactory.Create(new[] { new TestCollider2DComponentFactory() });
        Entity = Scene.CreateEntity();
    }

    [Test]
    public void Constructor_ShouldCreateColliderThatIsNotColliding()
    {
        // Arrange
        // Act
        var collider2D = Entity.CreateComponent<TestCollider2DComponent>();

        // Assert
        Assert.That(collider2D.IsColliding, Is.False);
        Assert.That(collider2D.CollidingEntities, Is.Empty);
    }

    [Test]
    public void Constructor_ShouldThrowException_WhenColliderIsAddedToEntityTwice()
    {
        // Arrange
        Entity.CreateComponent<TestCollider2DComponent>();

        // Act
        // Assert
        Assert.That(() => Entity.CreateComponent<TestCollider2DComponent>(), Throws.ArgumentException);
    }

    [Test]
    public void AddCollidingEntity_ShouldMakeEntityColliding()
    {
        // Arrange
        var collider2D = Entity.CreateComponent<TestCollider2DComponent>();
        var entity = Scene.CreateEntity();

        // Assume
        Assert.That(collider2D.IsColliding, Is.False);
        Assert.That(collider2D.CollidingEntities, Is.Empty);

        // Act
        collider2D.AddCollidingEntity(entity);

        // Assert
        Assert.That(collider2D.IsColliding, Is.True);
        Assert.That(collider2D.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(collider2D.CollidingEntities.Single(), Is.EqualTo(entity));
    }

    [Test]
    public void AddCollidingEntity_ShouldNotAddDuplicateEntities()
    {
        // Arrange
        var collider2D = Entity.CreateComponent<TestCollider2DComponent>();
        var entity = Scene.CreateEntity();

        // Assume
        Assert.That(collider2D.CollidingEntities, Is.Empty);

        // Act
        collider2D.AddCollidingEntity(entity);
        collider2D.AddCollidingEntity(entity);

        // Assert
        Assert.That(collider2D.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(collider2D.CollidingEntities.Single(), Is.EqualTo(entity));
    }

    [Test]
    public void ClearCollidingEntities_ShouldMakeEntityNotColliding()
    {
        // Arrange
        var collider2D = Entity.CreateComponent<TestCollider2DComponent>();
        var entity = Scene.CreateEntity();

        collider2D.AddCollidingEntity(entity);

        // Assume
        Assert.That(collider2D.IsColliding, Is.True);
        Assert.That(collider2D.CollidingEntities, Has.Count.EqualTo(1));
        Assert.That(collider2D.CollidingEntities.Single(), Is.EqualTo(entity));

        // Act
        collider2D.ClearCollidingEntities();

        // Assert
        Assert.That(collider2D.IsColliding, Is.False);
        Assert.That(collider2D.CollidingEntities, Is.Empty);
    }

    private sealed class TestCollider2DComponent : Collider2DComponent
    {
        public TestCollider2DComponent(Entity entity) : base(entity)
        {
        }
    }

    private sealed class TestCollider2DComponentFactory : ComponentFactory<TestCollider2DComponent>
    {
        protected override TestCollider2DComponent CreateComponent(Entity entity) => new(entity);
    }
}