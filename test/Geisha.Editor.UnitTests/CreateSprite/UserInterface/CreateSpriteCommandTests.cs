using Geisha.Editor.CreateSprite.Model;
using Geisha.Editor.CreateSprite.UserInterface;
using Geisha.Editor.ProjectHandling.Model;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateSprite.UserInterface
{
    [TestFixture]
    public class CreateSpriteCommandTests
    {
        private ICreateSpriteService _createSpriteService = null!;
        private IProjectFile _projectFile = null!;
        private CreateSpriteCommand _command = null!;

        [SetUp]
        public void SetUp()
        {
            _createSpriteService = Substitute.For<ICreateSpriteService>();
            _projectFile = Substitute.For<IProjectFile>();
            _command = new CreateSpriteCommand(_createSpriteService, _projectFile);
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
        public void Execute_ShouldRunCreateSpriteServiceWithProjectFile()
        {
            // Arrange
            // Act
            _command.Execute(null);

            // Assert
            _createSpriteService.Received().CreateSprite(_projectFile);
        }
    }
}