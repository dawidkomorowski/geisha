using Geisha.Engine.Core.Math;
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
        var linearVelocity = new Vector2(1, 2);
        var angularVelocity = 3.14;
        var enableCollisionResponse = true;

        // Act
        var actual = SerializeAndDeserialize<KinematicRigidBody2DComponent>(component =>
        {
            component.LinearVelocity = linearVelocity;
            component.AngularVelocity = angularVelocity;
            component.EnableCollisionResponse = enableCollisionResponse;
        });

        // Assert
        Assert.That(actual.LinearVelocity, Is.EqualTo(linearVelocity));
        Assert.That(actual.AngularVelocity, Is.EqualTo(angularVelocity));
        Assert.That(actual.EnableCollisionResponse, Is.EqualTo(enableCollisionResponse));
    }
}