using Geisha.Engine.Physics.Components;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Components;

[TestFixture]
public class KinematicRigidBody2DComponentSerializationTests : ComponentSerializationTestsBase
{
    [Test]
    public void SerializeAndDeserialize()
    {
        // Arrange
        // Act
        var actual = SerializeAndDeserialize<KinematicRigidBody2DComponent>(_ => { });

        // Assert
        Assert.That(actual, Is.Not.Null);
    }
}