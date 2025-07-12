using Geisha.Engine.Core.Math;
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

        // Act
        var actual = SerializeAndDeserialize<RectangleColliderComponent>(component => { component.Dimensions = dimensions; });

        // Assert
        Assert.That(actual.Dimensions, Is.EqualTo(dimensions));
        Assert.That(actual.IsColliding, Is.False);
        Assert.That(actual.GetContacts(), Is.Empty);
    }
}