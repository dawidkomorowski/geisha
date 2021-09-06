using Geisha.Editor.CreateSpriteAsset.Model;
using Geisha.Editor.CreateSpriteAsset.UserInterface;
using Geisha.Editor.ProjectHandling.Model;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateSpriteAsset.UserInterface
{
    [TestFixture]
    public class CreateSpriteAssetCommandTests
    {
        private ICreateSpriteAssetService _createSpriteAssetService = null!;
        private IProjectFile _projectFile = null!;
        private CreateSpriteAssetCommand _command = null!;

        [SetUp]
        public void SetUp()
        {
            _createSpriteAssetService = Substitute.For<ICreateSpriteAssetService>();
            _projectFile = Substitute.For<IProjectFile>();
            _command = new CreateSpriteAssetCommand(_createSpriteAssetService, _projectFile);
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
        public void Execute_ShouldRunCreateSpriteAssetServiceWithProjectFile()
        {
            // Arrange
            // Act
            _command.Execute(null);

            // Assert
            _createSpriteAssetService.Received().CreateSpriteAsset(_projectFile);
        }
    }
}