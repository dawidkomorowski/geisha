using Geisha.Engine.Core.SceneModel;
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
    public void Constructor_ShouldCreateColliderThatIsNotColliding()
    {
        // Arrange
        // Act
        var collider = _entity.CreateComponent<RectangleColliderComponent>();

        // Assert
        Assert.That(collider.IsColliding, Is.False);
        Assert.That(collider.GetContacts(), Is.Empty);
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
    public void Constructor_ShouldThrowException_WhenColliderIsAddedToEntityTwice()
    {
        // Arrange
        _entity.CreateComponent<RectangleColliderComponent>();

        // Act
        // Assert
        Assert.That(() => _entity.CreateComponent<RectangleColliderComponent>(), Throws.ArgumentException);
    }
}