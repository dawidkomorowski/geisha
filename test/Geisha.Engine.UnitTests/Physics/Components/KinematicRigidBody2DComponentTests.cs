using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Components;

[TestFixture]
public class KinematicRigidBody2DComponentTests
{
    private Scene Scene { get; set; } = null!;
    private Entity Entity { get; set; } = null!;

    [SetUp]
    public void SetUp()
    {
        Scene = TestSceneFactory.Create();
        Entity = Scene.CreateEntity();
    }

    [Test]
    public void Constructor_ShouldThrowException_WhenKinematicRigidBody2DComponentIsAddedToEntityTwice()
    {
        // Arrange
        Entity.CreateComponent<KinematicRigidBody2DComponent>();

        // Act
        // Assert
        Assert.That(() => Entity.CreateComponent<KinematicRigidBody2DComponent>(), Throws.ArgumentException);
    }
}