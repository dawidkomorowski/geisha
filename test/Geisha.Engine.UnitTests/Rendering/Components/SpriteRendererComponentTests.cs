using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components
{
    [TestFixture]
    public class SpriteRendererComponentTests
    {
        private Entity Entity { get; set; } = null!;

        [SetUp]
        public void SetUp()
        {
            var scene = TestSceneFactory.Create();
            Entity = scene.CreateEntity();
        }

        [TestCase(1d, 1d)]
        [TestCase(0d, 0d)]
        [TestCase(0.5d, 0.5d)]
        [TestCase(2d, 1d)]
        [TestCase(-1d, 0d)]
        public void Opacity_ShouldLimitTheValueToRangeFromZeroToOne(double opacity, double expected)
        {
            // Arrange
            var spriteRendererComponent = Entity.CreateComponent<SpriteRendererComponent>();

            // Act
            spriteRendererComponent.Opacity = opacity;

            // Assert
            Assert.That(spriteRendererComponent.Opacity, Is.EqualTo(expected));
        }
    }
}