using System;
using System.IO;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
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
        public void CreateSoundAsset_ShouldCreateSoundAssetFileInTheSameDirectoryAsSoundFileAndReturnItsPath_GivenSoundFilePath()
        {
            // Arrange
            var mp3FilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestSound.mp3");
            var mp3FilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestSound.mp3");
            File.Copy(mp3FilePathToCopy, mp3FilePathInTempDir);

            // Act
            var actual = AssetTool.CreateSoundAsset(mp3FilePathInTempDir);

            // Assert
            var soundAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestSound"));
            Assert.That(actual, Is.EqualTo(soundAssetFilePath));
            Assert.That(File.Exists(soundAssetFilePath), Is.True, "Sound asset file was not created.");

            var assetData = AssetData.Load(soundAssetFilePath);
            Assert.That(assetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(assetData.AssetType, Is.EqualTo(AudioAssetTypes.Sound));

            var soundAssetContent = assetData.ReadJsonContent<SoundAssetContent>();
            Assert.That(soundAssetContent.SoundFilePath, Is.EqualTo("TestSound.mp3"));
        }

        [Test]
        public void CreateSoundAsset_ShouldThrowException_GivenFilePathToUnsupportedSoundFile()
        {
            // Arrange
            var unsupportedFilePath = Path.Combine(_temporaryDirectory.Path, "TestSound.unsupported");
            File.WriteAllText(unsupportedFilePath, string.Empty);

            // Act
            // Assert
            Assert.That(() => AssetTool.CreateSoundAsset(unsupportedFilePath), Throws.ArgumentException);
        }

        [Test]
        public void CreateTextureAsset_ShouldCreateTextureAssetFileInTheSameDirectoryAsTextureFileAndReturnItsPath_GivenTextureFilePath()
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            // Act
            var actual = AssetTool.CreateTextureAsset(pngFilePathInTempDir);

            // Assert
            var textureAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestTexture"));
            Assert.That(actual, Is.EqualTo(textureAssetFilePath));
            Assert.That(File.Exists(textureAssetFilePath), Is.True, "Texture asset file was not created.");

            var assetData = AssetData.Load(textureAssetFilePath);
            Assert.That(assetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(assetData.AssetType, Is.EqualTo(RenderingAssetTypes.Texture));

            var textureAssetContent = assetData.ReadJsonContent<TextureAssetContent>();
            Assert.That(textureAssetContent.TextureFilePath, Is.EqualTo("TestTexture.png"));
        }

        [Test]
        public void CreateTextureAsset_ShouldThrowException_GivenFilePathToUnsupportedTextureFile()
        {
            // Arrange
            var unsupportedFilePath = Path.Combine(_temporaryDirectory.Path, "TestTexture.unsupported");
            File.WriteAllText(unsupportedFilePath, string.Empty);

            // Act
            // Assert
            Assert.That(() => AssetTool.CreateTextureAsset(unsupportedFilePath), Throws.ArgumentException);
        }
    }
}