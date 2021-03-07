using Geisha.Engine.Core.SceneModel;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel
{
    [TestFixture]
    public class ComponentFactoryProviderTests
    {
        [Test]
        public void Constructor_ThrowsArgumentException_GivenFactoriesWithDuplicatedComponentType()
        {
            // Arrange
            var factory1 = Substitute.For<IComponentFactory>();
            factory1.ComponentType.Returns(typeof(int));
            factory1.ComponentId.Returns(new ComponentId("Component 1"));

            var factory2 = Substitute.For<IComponentFactory>();
            factory2.ComponentType.Returns(typeof(int));
            factory2.ComponentId.Returns(new ComponentId("Component 2"));

            var factory3 = Substitute.For<IComponentFactory>();
            factory3.ComponentType.Returns(typeof(double));
            factory3.ComponentId.Returns(new ComponentId("Component 3"));

            // Act

            // Assert
            Assert.That(() => new ComponentFactoryProvider(new[] {factory1, factory2, factory3}),
                Throws.ArgumentException.With.Message.Contains(typeof(int).FullName));
        }

        [Test]
        public void Constructor_ThrowsArgumentException_GivenFactoriesWithDuplicatedComponentId()
        {
            // Arrange
            var factory1 = Substitute.For<IComponentFactory>();
            factory1.ComponentType.Returns(typeof(int));
            factory1.ComponentId.Returns(new ComponentId("Component 1"));

            var factory2 = Substitute.For<IComponentFactory>();
            factory2.ComponentType.Returns(typeof(float));
            factory2.ComponentId.Returns(new ComponentId("Component 1"));

            var factory3 = Substitute.For<IComponentFactory>();
            factory3.ComponentType.Returns(typeof(double));
            factory3.ComponentId.Returns(new ComponentId("Component 2"));

            // Act

            // Assert
            Assert.That(() => new ComponentFactoryProvider(new[] {factory1, factory2, factory3}),
                Throws.ArgumentException.With.Message.Contains("Component 1"));
        }
    }
}