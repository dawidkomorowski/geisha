using Geisha.Editor.CreateSound.Model;
using Geisha.Editor.CreateSound.UserInterface;
using Geisha.Editor.ProjectHandling.Model;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateSound.UserInterface
{
    [TestFixture]
    public class CreateSoundCommandTests
    {
        private ICreateSoundService _createSoundService = null!;
        private IProjectFile _projectFile = null!;
        private CreateSoundCommand _command = null!;

        [SetUp]
        public void SetUp()
        {
            _createSoundService = Substitute.For<ICreateSoundService>();
            _projectFile = Substitute.For<IProjectFile>();
            _command = new CreateSoundCommand(_createSoundService, _projectFile);
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
        public void Execute_ShouldRunCreateSoundServiceWithProjectFile()
        {
            // Arrange
            // Act
            _command.Execute(null);

            // Assert
            _createSoundService.Received().CreateSound(_projectFile);
        }
    }
}