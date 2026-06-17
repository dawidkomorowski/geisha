using Geisha.Engine.Physics;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Components;

public class TileColliderComponentSerializationTests : ComponentSerializationTestsBase
{
    [Test]
    public void SerializeAndDeserialize()
    {
        // Arrange
        const bool enabled = false;
        const bool isSensor = true;
        var collisionLayer = CollisionBitmask.FromBits(0, 2);
        var collisionMask = CollisionBitmask.FromBits(1, 3);

        // Act
        var actual = SerializeAndDeserialize<TileColliderComponent>(component =>
        {
            component.Enabled = enabled;
            component.IsSensor = isSensor;
            component.CollisionLayer = collisionLayer;
            component.CollisionMask = collisionMask;
        });

        // Assert
        Assert.That(actual.IsColliding, Is.False);
        Assert.That(actual.Enabled, Is.EqualTo(enabled));
        Assert.That(actual.IsSensor, Is.EqualTo(isSensor));
        Assert.That(actual.CollisionLayer, Is.EqualTo(collisionLayer));
        Assert.That(actual.CollisionMask, Is.EqualTo(collisionMask));
    }
}