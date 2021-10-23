using Geisha.Editor.CreateTextureAsset.Model;
using Geisha.Editor.CreateTextureAsset.UserInterface;
using Geisha.Editor.ProjectHandling.Model;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Editor.UnitTests.CreateTextureAsset.UserInterface
{
    [TestFixture]
    public class CreateTextureAssetCommandTests
    {
        private ICreateTextureAssetService _createTextureAssetService = null!;
        private IProjectFile _projectFile = null!;
        private CreateTextureAssetCommand _command = null!;

        [SetUp]
        public void SetUp()
        {
            _createTextureAssetService = Substitute.For<ICreateTextureAssetService>();
            _projectFile = Substitute.For<IProjectFile>();
            _command = new CreateTextureAssetCommand(_createTextureAssetService, _projectFile);
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
        public void Execute_ShouldRunCreateTextureAssetServiceWithProjectFile()
        {
            // Arrange
            // Act
            _command.Execute(null);

            // Assert
            _createTextureAssetService.Received().CreateTextureAsset(_projectFile);
        }
    }
}