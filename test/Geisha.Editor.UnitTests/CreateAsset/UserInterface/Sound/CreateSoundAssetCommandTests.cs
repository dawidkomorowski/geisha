using Geisha.Editor.CreateAsset.Model;
using Geisha.Editor.CreateAsset.UserInterface.Sound;
using Geisha.Editor.ProjectHandling.Model;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateAsset.UserInterface.Sound
{
    [TestFixture]
    public class CreateSoundAssetCommandTests
    {
        private ICreateSoundAssetService _createSoundAssetService = null!;
        private IProjectFile _projectFile = null!;
        private CreateSoundAssetCommand _command = null!;

        [SetUp]
        public void SetUp()
        {
            _createSoundAssetService = Substitute.For<ICreateSoundAssetService>();
            _projectFile = Substitute.For<IProjectFile>();
            _command = new CreateSoundAssetCommand(_createSoundAssetService, _projectFile);
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
        public void Execute_ShouldRunCreateSoundAssetServiceWithProjectFile()
        {
            // Arrange
            // Act
            _command.Execute(null);

            // Assert
            _createSoundAssetService.Received().CreateSoundAsset(_projectFile);
        }
    }
}