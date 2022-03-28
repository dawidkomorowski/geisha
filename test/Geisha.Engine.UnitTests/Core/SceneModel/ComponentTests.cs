using System;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel
{
    [TestFixture]
    public class ComponentTests
    {
        private Entity Entity { get; set; } = null!;

        [SetUp]
        public void SetUp()
        {
            var scene = TestSceneFactory.Create(new IComponentFactory[] { new ComponentWithoutCustomIdFactory(), new ComponentWithCustomIdFactory() });
            Entity = scene.CreateEntity();
        }

        [Test]
        public void ComponentId_ShouldReturnComponentIdEqualFullNameOfComponentType_WhenComponentIdAttributeIsNotApplied()
        {
            // Arrange
            var component = Entity.CreateComponent<ComponentWithoutCustomId>();

            // Act
            // Assert
            Assert.That(component.ComponentId, Is.EqualTo(new ComponentId(typeof(ComponentWithoutCustomId).FullName ?? throw new InvalidOperationException())));
        }

        [Test]
        public void ComponentId_ShouldReturnComponentIdEqualComponentIdAttribute_WhenComponentIdAttributeIsApplied()
        {
            // Arrange
            var component = Entity.CreateComponent<ComponentWithCustomId>();

            // Act
            // Assert
            Assert.That(component.ComponentId, Is.EqualTo(new ComponentId("Custom Component Id")));
        }

        private sealed class ComponentWithoutCustomId : Component
        {
            public ComponentWithoutCustomId(Entity entity) : base(entity)
            {
            }
        }

        private sealed class ComponentWithoutCustomIdFactory : ComponentFactory<ComponentWithoutCustomId>
        {
            protected override ComponentWithoutCustomId CreateComponent(Entity entity) => new ComponentWithoutCustomId(entity);
        }

        [ComponentId("Custom Component Id")]
        private sealed class ComponentWithCustomId : Component
        {
            public ComponentWithCustomId(Entity entity) : base(entity)
            {
            }
        }

        private sealed class ComponentWithCustomIdFactory : ComponentFactory<ComponentWithCustomId>
        {
            protected override ComponentWithCustomId CreateComponent(Entity entity) => new ComponentWithCustomId(entity);
        }
    }
}