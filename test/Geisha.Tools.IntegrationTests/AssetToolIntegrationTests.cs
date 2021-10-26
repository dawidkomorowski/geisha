using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Tools.IntegrationTests
{
    [TestFixture]
    public class AssetToolIntegrationTests
    {
        private TemporaryDirectory _temporaryDirectory = null!;

        [SetUp]
        public void SetUp()
        {
            _temporaryDirectory = new TemporaryDirectory();
        }

        [TearDown]
        public void TearDown()
        {
            _temporaryDirectory.Dispose();
        }

        [Test]
        public void CreateSoundAsset_ShouldCreateSoundAssetFileInTheSameFolderAsSoundFileAndReturnItsPath_GivenSoundFilePath()
        {
            // Arrange
            // Act
            var actual = AssetTool.CreateSoundAsset("some path");

            // Assert
        }
    }
}