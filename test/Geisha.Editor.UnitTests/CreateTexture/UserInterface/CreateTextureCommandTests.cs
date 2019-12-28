using Geisha.Editor.CreateTexture.Model;
using Geisha.Editor.CreateTexture.UserInterface;
using Geisha.Editor.ProjectHandling.Model;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateTexture.UserInterface
{
    [TestFixture]
    public class CreateTextureCommandTests
    {
        private ICreateTextureService _createTextureService;
        private IProjectFile _projectFile;
        private CreateTextureCommand _command;

        [SetUp]
        public void SetUp()
        {
            _createTextureService = Substitute.For<ICreateTextureService>();
            _projectFile = Substitute.For<IProjectFile>();
            _command = new CreateTextureCommand(_createTextureService, _projectFile);
        }

        [Test]
        public void CanExecute_ShouldReturnTrue()
        {
            // Arrange
            // Act
            var actual = _command.CanExecute(null);

            // Assert
            Assert.That(actual, Is.True);
        }

        [Test]
        public void Execute_ShouldRunCreateTextureServiceWithProjectFile()
        {
            // Arrange
            // Act
            _command.Execute(null);

            // Assert
            _createTextureService.Received().CreateTexture(_projectFile);
        }
    }
}