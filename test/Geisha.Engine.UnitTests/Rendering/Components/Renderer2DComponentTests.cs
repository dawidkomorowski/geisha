using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components
{
    [TestFixture]
    public class Renderer2DComponentTests
    {
        private Entity Entity { get; set; } = null!;

        [SetUp]
        public void SetUp()
        {
            var scene = TestSceneFactory.Create();
            Entity = scene.CreateEntity();
        }

        [Test]
        public void Constructor_ShouldThrowException_WhenRenderer2DComponentIsAddedToEntityWithRenderer2DComponent()
        {
            // Arrange
            Entity.CreateComponent<SpriteRendererComponent>();

            // Act
            // Assert
            Assert.That(() => Entity.CreateComponent<SpriteRendererComponent>(), Throws.ArgumentException);
        }
    }
}