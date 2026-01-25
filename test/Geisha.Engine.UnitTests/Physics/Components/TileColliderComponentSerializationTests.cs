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

        // Act
        var actual = SerializeAndDeserialize<TileColliderComponent>(component => { component.Enabled = enabled; });

        // Assert
        Assert.That(actual.IsColliding, Is.False);
        Assert.That(actual.GetContacts(), Is.Empty);
        Assert.That(actual.Enabled, Is.EqualTo(enabled));
    }
}