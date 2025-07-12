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

        // Act
        var actual = SerializeAndDeserialize<CircleColliderComponent>(component => { component.Radius = radius; });

        // Assert
        Assert.That(actual.Radius, Is.EqualTo(radius));
        Assert.That(actual.IsColliding, Is.False);
        Assert.That(actual.GetContacts(), Is.Empty);
    }
}