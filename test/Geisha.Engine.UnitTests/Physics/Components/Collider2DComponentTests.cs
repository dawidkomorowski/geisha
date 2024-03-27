using System.Linq;
using Geisha.Engine.Core.Collections;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Components;

[TestFixture]
public class Collider2DComponentTests
{
    private Scene _scene = null!;
    private Entity _entity = null!;

    [SetUp]
    public void SetUp()
    {
        _scene = TestSceneFactory.Create(new[] { new TestCollider2DComponentFactory() });
        _entity = _scene.CreateEntity();
    }

    [Test]
    public void Constructor_ShouldCreateColliderThatIsNotColliding()
    {
        // Arrange
        // Act
        var collider = _entity.CreateComponent<TestCollider2DComponent>();

        // Assert
        Assert.That(collider.IsColliding, Is.False);
        Assert.That(collider.Contacts, Is.Empty);
    }

    [Test]
    public void Constructor_ShouldThrowException_WhenColliderIsAddedToEntityTwice()
    {
        // Arrange
        _entity.CreateComponent<TestCollider2DComponent>();

        // Act
        // Assert
        Assert.That(() => _entity.CreateComponent<TestCollider2DComponent>(), Throws.ArgumentException);
    }

    [Test]
    public void AddContact_ShouldMakeEntityColliding()
    {
        // Arrange
        var collider = _entity.CreateComponent<TestCollider2DComponent>();
        var otherEntity = _scene.CreateEntity();
        var otherCollider = otherEntity.CreateComponent<TestCollider2DComponent>();
        var contact = CreateContact(collider, otherCollider);

        // Assume
        Assert.That(collider.IsColliding, Is.False);
        Assert.That(collider.Contacts, Is.Empty);

        // Act
        collider.AddContact(contact);

        // Assert
        Assert.That(collider.IsColliding, Is.True);
        Assert.That(collider.Contacts, Has.Count.EqualTo(1));
        Assert.That(collider.Contacts.Single(), Is.EqualTo(contact));
    }

    [Test]
    public void ClearContacts_ShouldMakeEntityNotColliding()
    {
        // Arrange
        var collider = _entity.CreateComponent<TestCollider2DComponent>();
        var otherEntity = _scene.CreateEntity();
        var otherCollider = otherEntity.CreateComponent<TestCollider2DComponent>();
        var contact = CreateContact(collider, otherCollider);

        collider.AddContact(contact);

        // Assume
        Assert.That(collider.IsColliding, Is.True);
        Assert.That(collider.Contacts, Has.Count.EqualTo(1));
        Assert.That(collider.Contacts.Single(), Is.EqualTo(contact));

        // Act
        collider.ClearContacts();

        // Assert
        Assert.That(collider.IsColliding, Is.False);
        Assert.That(collider.Contacts, Is.Empty);
    }

    private static Contact2D CreateContact(Collider2DComponent thisCollider, Collider2DComponent otherCollider)
    {
        var contactPoints = new ReadOnlyFixedList2<ContactPoint2D>(default);
        return new Contact2D(thisCollider, otherCollider, Vector2.UnitX, 5, contactPoints);
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