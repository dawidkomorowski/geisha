using Geisha.Engine.Rendering.Configuration;
using NUnit.Framework;

namespace Geisha.Engine.Rendering.UnitTests.Configuration
{
    [TestFixture]
    public class RenderingDefaultConfigurationFactoryTests
    {
        [Test]
        public void CreateDefault_ShouldReturnNotNull()
        {
            // Arrange
            var defaultConfigurationFactory = new RenderingDefaultConfigurationFactory();

            // Act
            var configuration = defaultConfigurationFactory.CreateDefault();

            // Assert
            Assert.That(configuration, Is.Not.Null);
        }

        [Test]
        public void CreateDefault_ShouldReturnConfigurationOfDeclaredType()
        {
            // Arrange
            var defaultConfigurationFactory = new RenderingDefaultConfigurationFactory();

            // Act
            var configuration = defaultConfigurationFactory.CreateDefault();

            // Assert
            Assert.That(configuration, Is.TypeOf(defaultConfigurationFactory.ConfigurationType));
        }
    }
}