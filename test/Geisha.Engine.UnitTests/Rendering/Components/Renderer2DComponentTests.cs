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
            var scene = TestSceneFactory.Create(new[] { new CustomRenderer2DComponentFactory() });
            Entity = scene.CreateEntity();
        }

        [Test]
        public void Constructor_ShouldThrowException_WhenRenderer2DComponentIsAddedToEntityWithRenderer2DComponent()
        {
            // Arrange
            Entity.CreateComponent<CustomRenderer2DComponent>();

            // Act
            // Assert
            Assert.That(() => Entity.CreateComponent<CustomRenderer2DComponent>(), Throws.ArgumentException);
        }

        private sealed class CustomRenderer2DComponent : Renderer2DComponent
        {
            public CustomRenderer2DComponent(Entity entity) : base(entity)
            {
            }
        }

        private sealed class CustomRenderer2DComponentFactory : ComponentFactory<CustomRenderer2DComponent>
        {
            protected override CustomRenderer2DComponent CreateComponent(Entity entity) => new(entity);
        }
    }
}