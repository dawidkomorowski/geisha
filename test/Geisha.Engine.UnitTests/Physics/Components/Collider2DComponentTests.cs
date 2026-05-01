using System;
using System.Collections.Generic;
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
        _scene = TestSceneFactory.Create();
        _entity = _scene.CreateEntity();
    }

    [Test]
    public void Constructor_ShouldThrowException_WhenColliderIsAddedToEntityTwice()
    {
        // Arrange
        _entity.CreateComponent<RectangleColliderComponent>();

        // Act
        // Assert
        Assert.That(() => _entity.CreateComponent<RectangleColliderComponent>(), Throws.ArgumentException);
    }

    [Test]
    public void Constructor_ShouldCreateColliderThatIsEnabled()
    {
        // Arrange
        // Act
        var collider = _entity.CreateComponent<RectangleColliderComponent>();

        // Assert
        Assert.That(collider.Enabled, Is.True);
    }

    [Test]
    public void AllPhysicsAPIs_ShouldReturnDefaults_WhenNotManagedByPhysicsSystem()
    {
        // Arrange
        // Act
        var collider = _entity.CreateComponent<RectangleColliderComponent>();

        // Assert
        Assert.That(collider.BoundingRectangle, Is.EqualTo(default(AxisAlignedRectangle)));
        Assert.That(collider.IsColliding, Is.False);
        Assert.That(collider.ContactCount, Is.Zero);

        // GetContacts should return 0 contacts
        var contactsSpan = new Contact2D[10];
        Assert.That(collider.GetContacts(contactsSpan.AsSpan()), Is.Zero);

        // GetContacts with list should return 0 contacts
        var contactsList = new List<Contact2D>();
        Assert.That(collider.GetContacts(contactsList), Is.Zero);
        Assert.That(contactsList, Is.Empty);

        // GetContactsAsSpan should return empty span
        var contactsSpanView = collider.GetContactsAsSpan(contactsSpan.AsSpan());
        Assert.That(contactsSpanView.Length, Is.Zero);

        // GetContactsAsSpan with list should return empty span
        var contactsListView = collider.GetContactsAsSpan(contactsList);
        Assert.That(contactsListView.Length, Is.Zero);
    }
}