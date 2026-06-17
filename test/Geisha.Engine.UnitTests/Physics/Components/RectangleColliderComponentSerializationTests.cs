using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Components;

[TestFixture]
public class RectangleColliderComponentSerializationTests : ComponentSerializationTestsBase
{
    [Test]
    public void SerializeAndDeserialize()
    {
        // Arrange
        var dimensions = new Vector2(12.34, 56.78);
        const bool enabled = false;
        const bool isSensor = true;
        var collisionLayer = CollisionBitmask.FromBits(0, 2);
        var collisionMask = CollisionBitmask.FromBits(1, 3);

        // Act
        var actual = SerializeAndDeserialize<RectangleColliderComponent>(component =>
        {
            component.Dimensions = dimensions;
            component.Enabled = enabled;
            component.IsSensor = isSensor;
            component.CollisionLayer = collisionLayer;
            component.CollisionMask = collisionMask;
        });

        // Assert
        Assert.That(actual.Dimensions, Is.EqualTo(dimensions));
        Assert.That(actual.IsColliding, Is.False);
        Assert.That(actual.Enabled, Is.EqualTo(enabled));
        Assert.That(actual.IsSensor, Is.EqualTo(isSensor));
        Assert.That(actual.CollisionLayer, Is.EqualTo(collisionLayer));
        Assert.That(actual.CollisionMask, Is.EqualTo(collisionMask));
    }
}