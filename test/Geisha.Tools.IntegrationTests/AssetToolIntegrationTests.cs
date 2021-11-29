using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Assets;
using Geisha.Engine.Input.Assets.Serialization;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
using Geisha.IntegrationTestsData;
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
        public void CreateTextureAsset_ShouldThrowException_GivenFilePathToUnsupportedTextureFile()
        {
            // Arrange
            var unsupportedFilePath = Path.Combine(_temporaryDirectory.Path, "TestTexture.unsupported");
            File.WriteAllText(unsupportedFilePath, string.Empty);

            // Act
            // Assert
            Assert.That(() => AssetTool.CreateTextureAsset(unsupportedFilePath), Throws.ArgumentException);
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

        [TestCase("TestSound.mp3", false)]
        [TestCase("TestSound.geisha-asset", false)]
        [TestCase("TestTexture.geisha-asset", true)]
        [TestCase("TestTexture.png", true)]
        public void CanCreateSpriteAssetFromFile_ShouldReturnTrue_WhenFileIsEitherTextureAssetFileOrTextureFile(string fileName, bool expected)
        {
            // Arrange
            var filePath = Utils.GetPathUnderTestDirectory(Path.Combine("Assets", fileName));

            // Act
            var actual = AssetTool.CanCreateSpriteAssetFromFile(filePath);

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void CreateSpriteAsset_ShouldThrowException_GivenFilePathToNeitherTextureAssetFileNorTextureFile()
        {
            // Arrange
            var unsupportedFilePath = Path.Combine(_temporaryDirectory.Path, "TestTexture.unsupported");
            File.WriteAllText(unsupportedFilePath, string.Empty);

            // Act
            // Assert
            Assert.That(() => AssetTool.CreateSpriteAsset(unsupportedFilePath), Throws.ArgumentException);
        }

        [Test]
        public void CreateSpriteAsset_ShouldCreateSpriteAssetFileInTheSameDirectoryAsTextureAssetFile_GivenTextureAssetFilePath()
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            var textureAssetFilePathToCopy = Utils.GetPathUnderTestDirectory(AssetFileUtils.AppendExtension(@"Assets\TestTexture"));
            var textureAssetFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestTexture"));
            File.Copy(textureAssetFilePathToCopy, textureAssetFilePathInTempDir);

            // Act
            var actual = AssetTool.CreateSpriteAsset(textureAssetFilePathInTempDir);

            // Assert
            var spriteAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestTexture.sprite"));
            Assert.That(actual.spriteAssetFilePath, Is.EqualTo(spriteAssetFilePath));
            Assert.That(actual.textureAssetFilePath, Is.Null);
            Assert.That(File.Exists(spriteAssetFilePath), Is.True, "Sprite asset file was not created.");

            var assetData = AssetData.Load(spriteAssetFilePath);
            Assert.That(assetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(assetData.AssetType, Is.EqualTo(RenderingAssetTypes.Sprite));
            var spriteAssetContent = assetData.ReadJsonContent<SpriteAssetContent>();
            Assert.That(spriteAssetContent.TextureAssetId, Is.EqualTo(AssetsIds.TestTexture.Value));
            Assert.That(spriteAssetContent.SourceUV.X, Is.Zero);
            Assert.That(spriteAssetContent.SourceUV.Y, Is.Zero);
            Assert.That(spriteAssetContent.SourceDimension.X, Is.EqualTo(10));
            Assert.That(spriteAssetContent.SourceDimension.Y, Is.EqualTo(10));
            Assert.That(spriteAssetContent.SourceAnchor.X, Is.EqualTo(5));
            Assert.That(spriteAssetContent.SourceAnchor.Y, Is.EqualTo(5));
            Assert.That(spriteAssetContent.PixelsPerUnit, Is.EqualTo(1));
        }

        [Test]
        public void CreateSpriteAsset_ShouldCreateTextureAssetFileAndSpriteAssetFileInTheSameFolderAsTextureFile_GivenTextureFilePath()
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            // Act
            var actual = AssetTool.CreateSpriteAsset(pngFilePathInTempDir);

            // Assert
            var textureAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestTexture"));
            Assert.That(actual.textureAssetFilePath, Is.EqualTo(textureAssetFilePath));
            Assert.That(File.Exists(textureAssetFilePath), Is.True, "Texture asset file was not created.");

            var textureAssetData = AssetData.Load(textureAssetFilePath);
            Assert.That(textureAssetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(textureAssetData.AssetType, Is.EqualTo(RenderingAssetTypes.Texture));
            var textureAssetContent = textureAssetData.ReadJsonContent<TextureAssetContent>();
            Assert.That(textureAssetContent.TextureFilePath, Is.EqualTo("TestTexture.png"));

            var spriteAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestTexture.sprite"));
            Assert.That(actual.spriteAssetFilePath, Is.EqualTo(spriteAssetFilePath));
            Assert.That(File.Exists(spriteAssetFilePath), Is.True, "Sprite asset file was not created.");

            var spriteAssetData = AssetData.Load(spriteAssetFilePath);
            Assert.That(spriteAssetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(spriteAssetData.AssetType, Is.EqualTo(RenderingAssetTypes.Sprite));
            var spriteAssetContent = spriteAssetData.ReadJsonContent<SpriteAssetContent>();
            Assert.That(spriteAssetContent.TextureAssetId, Is.EqualTo(textureAssetData.AssetId.Value));
            Assert.That(spriteAssetContent.SourceUV.X, Is.Zero);
            Assert.That(spriteAssetContent.SourceUV.Y, Is.Zero);
            Assert.That(spriteAssetContent.SourceDimension.X, Is.EqualTo(10));
            Assert.That(spriteAssetContent.SourceDimension.Y, Is.EqualTo(10));
            Assert.That(spriteAssetContent.SourceAnchor.X, Is.EqualTo(5));
            Assert.That(spriteAssetContent.SourceAnchor.Y, Is.EqualTo(5));
            Assert.That(spriteAssetContent.PixelsPerUnit, Is.EqualTo(1));
        }

        [Test]
        public void CreateInputMappingAsset_ShouldCreateDefaultInputMappingAssetFileInCurrentDirectory()
        {
            // Arrange
            Environment.CurrentDirectory = _temporaryDirectory.Path;

            // Act
            var actual = AssetTool.CreateInputMappingAsset();

            // Assert
            var inputMappingAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("DefaultInputMapping"));
            Assert.That(actual, Is.EqualTo(inputMappingAssetFilePath));
            Assert.That(File.Exists(inputMappingAssetFilePath), Is.True, "InputMapping asset file was not created.");

            var inputMappingAssetData = AssetData.Load(inputMappingAssetFilePath);
            Assert.That(inputMappingAssetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(inputMappingAssetData.AssetType, Is.EqualTo(InputAssetTypes.InputMapping));

            var inputMappingAssetContent = inputMappingAssetData.ReadJsonContent<InputMappingAssetContent>();

            Assert.That(inputMappingAssetContent.ActionMappings, Contains.Key("Jump"));
            Debug.Assert(inputMappingAssetContent.ActionMappings != null, "inputMappingAssetContent.ActionMappings != null");
            var jumpAction = inputMappingAssetContent.ActionMappings["Jump"];
            Assert.That(jumpAction, Has.Length.EqualTo(2));
            Assert.That(jumpAction[0].Key, Is.EqualTo(Key.Space));
            Assert.That(jumpAction[1].Key, Is.EqualTo(Key.Up));

            Assert.That(inputMappingAssetContent.AxisMappings, Contains.Key("MoveRight"));
            Debug.Assert(inputMappingAssetContent.AxisMappings != null, "inputMappingAssetContent.AxisMappings != null");
            var moveRightAxis = inputMappingAssetContent.AxisMappings["MoveRight"];
            Assert.That(moveRightAxis, Has.Length.EqualTo(2));
            Assert.That(moveRightAxis[0].Key, Is.EqualTo(Key.Right));
            Assert.That(moveRightAxis[0].Scale, Is.EqualTo(1.0));
            Assert.That(moveRightAxis[1].Key, Is.EqualTo(Key.Left));
            Assert.That(moveRightAxis[1].Scale, Is.EqualTo(-1.0));
        }
    }
}