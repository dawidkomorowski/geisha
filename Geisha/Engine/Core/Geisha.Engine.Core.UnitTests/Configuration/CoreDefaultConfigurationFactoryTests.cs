using Geisha.Engine.Core.Configuration;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Configuration
{
    [TestFixture]
    public class CoreDefaultConfigurationFactoryTests
    {
        [Test]
        public void CreateDefault_ShouldReturnNotNull()
        {
            // Arrange
            var defaultConfigurationFactory = new CoreDefaultConfigurationFactory();

            // Act
            var configuration = defaultConfigurationFactory.CreateDefault();

            // Assert
            Assert.That(configuration, Is.Not.Null);
        }

        [Test]
        public void CreateDefault_ShouldReturnConfigurationOfDeclaredType()
        {
            // Arrange
            var defaultConfigurationFactory = new CoreDefaultConfigurationFactory();

            // Act
            var configuration = defaultConfigurationFactory.CreateDefault();

            // Assert
            Assert.That(configuration, Is.TypeOf(defaultConfigurationFactory.ConfigurationType));
        }
    }
}