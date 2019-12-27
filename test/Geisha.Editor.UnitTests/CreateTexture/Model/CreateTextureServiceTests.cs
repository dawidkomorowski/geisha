using Geisha.Editor.CreateTexture.Model;
using Geisha.Editor.ProjectHandling.Model;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateTexture.Model
{
    [TestFixture]
    public class CreateTextureServiceTests
    {
        [Test]
        public void CreateTexture_ShouldNotThrow()
        {
            // Arrange
            var sourceTextureFile = Substitute.For<IProjectFile>();
            var createTextureService = new CreateTextureService(sourceTextureFile);

            // Act
            // Assert
            Assert.That(() => createTextureService.CreateTexture(), Throws.Nothing);
        }
    }
}