using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Components;

[TestFixture]
public class CircleColliderComponentSerializationTests : ComponentSerializationTestsBase
{
    [Test]
    public void SerializeAndDeserialize()
    {
        // Arrange
        const double radius = 123.456;
        const bool enabled = false;
        var collisionLayer = CollisionBitmask.FromBits(0, 2);
        var collisionMask = CollisionBitmask.FromBits(1, 3);

        // Act
        var actual = SerializeAndDeserialize<CircleColliderComponent>(component =>
        {
            component.Radius = radius;
            component.Enabled = enabled;
            component.CollisionLayer = collisionLayer;
            component.CollisionMask = collisionMask;
        });

        // Assert
        Assert.That(actual.Radius, Is.EqualTo(radius));
        Assert.That(actual.IsColliding, Is.False);
        Assert.That(actual.Enabled, Is.EqualTo(enabled));
        Assert.That(actual.CollisionLayer, Is.EqualTo(collisionLayer));
        Assert.That(actual.CollisionMask, Is.EqualTo(collisionMask));
    }
}