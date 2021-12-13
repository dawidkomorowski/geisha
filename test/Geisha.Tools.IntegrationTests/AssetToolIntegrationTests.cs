using System;
using System.Diagnostics;
using System.IO;
using Geisha.Engine.Animation.Assets;
using Geisha.Engine.Animation.Assets.Serialization;
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

        [TestCase(false)]
        [TestCase(true)]
        public void CreateSoundAsset_ShouldCreateSoundAssetFileInTheSameDirectoryAsSoundFileAndReturnItsPath_GivenSoundFilePath(bool keepAssetId)
        {
            // Arrange
            var mp3FilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestSound.mp3");
            var mp3FilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestSound.mp3");
            File.Copy(mp3FilePathToCopy, mp3FilePathInTempDir);

            // Act
            var actual = AssetTool.CreateSoundAsset(mp3FilePathInTempDir, keepAssetId);

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

        [TestCase(false)]
        [TestCase(true)]
        public void CreateSoundAsset_ShouldRecreateSoundAssetFileWithTheSameAssetId_WhenSoundAssetFileAlreadyExists_GivenKeepAssetId(bool keepAssetId)
        {
            // Arrange
            var mp3FilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestSound.mp3");
            var mp3FilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestSound.mp3");
            File.Copy(mp3FilePathToCopy, mp3FilePathInTempDir);

            var originalAssetFilePath = AssetTool.CreateSoundAsset(mp3FilePathInTempDir);

            var originalAssetData = AssetData.Load(originalAssetFilePath);
            var modifiedAssetData = AssetData.CreateWithJsonContent(originalAssetData.AssetId, originalAssetData.AssetType, new SoundAssetContent());
            modifiedAssetData.Save(originalAssetFilePath);

            // Act
            var actualAssetFilePath = AssetTool.CreateSoundAsset(mp3FilePathInTempDir, keepAssetId);

            // Assert
            var actualAssetData = AssetData.Load(actualAssetFilePath);

            Assert.That(actualAssetFilePath, Is.EqualTo(originalAssetFilePath));
            Assert.That(actualAssetData.AssetId, keepAssetId ? Is.EqualTo(originalAssetData.AssetId) : Is.Not.EqualTo(originalAssetData.AssetId));
            Assert.That(actualAssetData.ReadJsonContent<SoundAssetContent>().SoundFilePath, Is.EqualTo("TestSound.mp3"));
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

        [TestCase(false)]
        [TestCase(true)]
        public void CreateTextureAsset_ShouldCreateTextureAssetFileInTheSameDirectoryAsTextureFileAndReturnItsPath_GivenTextureFilePath(bool keepAssetId)
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            // Act
            var actual = AssetTool.CreateTextureAsset(pngFilePathInTempDir, keepAssetId);

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

        [TestCase(false)]
        [TestCase(true)]
        public void CreateTextureAsset_ShouldRecreateTextureAssetFileWithTheSameAssetId_WhenTextureAssetFileAlreadyExists_GivenKeepAssetId(bool keepAssetId)
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            var originalAssetFilePath = AssetTool.CreateTextureAsset(pngFilePathInTempDir);

            var originalAssetData = AssetData.Load(originalAssetFilePath);
            var modifiedAssetData = AssetData.CreateWithJsonContent(originalAssetData.AssetId, originalAssetData.AssetType, new TextureAssetContent());
            modifiedAssetData.Save(originalAssetFilePath);

            // Act
            var actualAssetFilePath = AssetTool.CreateTextureAsset(pngFilePathInTempDir, keepAssetId);

            // Assert
            var actualAssetData = AssetData.Load(actualAssetFilePath);

            Assert.That(actualAssetFilePath, Is.EqualTo(originalAssetFilePath));
            Assert.That(actualAssetData.AssetId, keepAssetId ? Is.EqualTo(originalAssetData.AssetId) : Is.Not.EqualTo(originalAssetData.AssetId));
            Assert.That(actualAssetData.ReadJsonContent<TextureAssetContent>().TextureFilePath, Is.EqualTo("TestTexture.png"));
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

        [TestCase(false)]
        [TestCase(true)]
        public void CreateSpriteAsset_ShouldCreateSpriteAssetFileInTheSameDirectoryAsTextureAssetFile_GivenTextureAssetFilePath(bool keepAssetId)
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            var textureAssetFilePathToCopy = Utils.GetPathUnderTestDirectory(AssetFileUtils.AppendExtension(@"Assets\TestTexture"));
            var textureAssetFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestTexture"));
            File.Copy(textureAssetFilePathToCopy, textureAssetFilePathInTempDir);

            // Act
            var actual = AssetTool.CreateSpriteAsset(textureAssetFilePathInTempDir, keepAssetId);

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

        [TestCase(false)]
        [TestCase(true)]
        public void
            CreateSpriteAsset_ShouldRecreateSpriteAssetFileWithTheSameAssetId_WhenSpriteAssetFileAlreadyExists_GivenTextureAssetFilePath_And_KeepAssetId(
                bool keepAssetId)
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            var textureAssetFilePathToCopy = Utils.GetPathUnderTestDirectory(AssetFileUtils.AppendExtension(@"Assets\TestTexture"));
            var textureAssetFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestTexture"));
            File.Copy(textureAssetFilePathToCopy, textureAssetFilePathInTempDir);

            var originalAssetFilePath = AssetTool.CreateSpriteAsset(textureAssetFilePathInTempDir).spriteAssetFilePath;

            var originalAssetData = AssetData.Load(originalAssetFilePath);
            var modifiedAssetData = AssetData.CreateWithJsonContent(originalAssetData.AssetId, originalAssetData.AssetType, new SpriteAssetContent());
            modifiedAssetData.Save(originalAssetFilePath);

            // Act
            var actualAssetFilePath = AssetTool.CreateSpriteAsset(textureAssetFilePathInTempDir, keepAssetId).spriteAssetFilePath;

            // Assert
            var actualAssetData = AssetData.Load(actualAssetFilePath);

            Assert.That(actualAssetFilePath, Is.EqualTo(originalAssetFilePath));
            Assert.That(actualAssetData.AssetId, keepAssetId ? Is.EqualTo(originalAssetData.AssetId) : Is.Not.EqualTo(originalAssetData.AssetId));
            Assert.That(actualAssetData.ReadJsonContent<SpriteAssetContent>().TextureAssetId, Is.EqualTo(AssetsIds.TestTexture.Value));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void CreateSpriteAsset_ShouldCreateTextureAssetFileAndSpriteAssetFileInTheSameFolderAsTextureFile_GivenTextureFilePath(bool keepAssetId)
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            // Act
            var actual = AssetTool.CreateSpriteAsset(pngFilePathInTempDir, keepAssetId);

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

        [TestCase(false)]
        [TestCase(true)]
        public void
            CreateSpriteAsset_ShouldRecreateTextureAssetFileAndSpriteAssetFileWithTheSameAssetId_WhenTextureAssetFileAndSpriteAssetFileAlreadyExist_GivenTextureFilePath_And_KeepAssetId(
                bool keepAssetId)
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            var (originalSpriteAssetFilePath, originalTextureAssetFilePath) = AssetTool.CreateSpriteAsset(pngFilePathInTempDir);

            var originalTextureAssetData = AssetData.Load(originalTextureAssetFilePath);
            var modifiedTextureAssetData =
                AssetData.CreateWithJsonContent(originalTextureAssetData.AssetId, originalTextureAssetData.AssetType, new TextureAssetContent());
            modifiedTextureAssetData.Save(originalTextureAssetFilePath);

            var originalSpriteAssetData = AssetData.Load(originalSpriteAssetFilePath);
            var modifiedSpriteAssetData =
                AssetData.CreateWithJsonContent(originalSpriteAssetData.AssetId, originalSpriteAssetData.AssetType, new SpriteAssetContent());
            modifiedSpriteAssetData.Save(originalSpriteAssetFilePath);

            // Act
            var (actualSpriteAssetFilePath, actualTextureAssetFilePath) = AssetTool.CreateSpriteAsset(pngFilePathInTempDir, keepAssetId);

            // Assert
            var actualTextureAssetData = AssetData.Load(actualTextureAssetFilePath);

            Assert.That(actualTextureAssetFilePath, Is.EqualTo(originalTextureAssetFilePath));
            Assert.That(actualTextureAssetData.AssetId,
                keepAssetId ? Is.EqualTo(originalTextureAssetData.AssetId) : Is.Not.EqualTo(originalTextureAssetData.AssetId));
            Assert.That(actualTextureAssetData.ReadJsonContent<TextureAssetContent>().TextureFilePath, Is.EqualTo("TestTexture.png"));

            var actualSpriteAssetData = AssetData.Load(actualSpriteAssetFilePath);

            Assert.That(actualSpriteAssetFilePath, Is.EqualTo(originalSpriteAssetFilePath));
            Assert.That(actualSpriteAssetData.AssetId,
                keepAssetId ? Is.EqualTo(originalSpriteAssetData.AssetId) : Is.Not.EqualTo(originalSpriteAssetData.AssetId));
            Assert.That(actualSpriteAssetData.ReadJsonContent<SpriteAssetContent>().TextureAssetId, Is.EqualTo(actualTextureAssetData.AssetId.Value));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void CreateInputMappingAsset_ShouldCreateDefaultInputMappingAssetFileInCurrentDirectory(bool keepAssetId)
        {
            // Arrange
            Environment.CurrentDirectory = _temporaryDirectory.Path;

            // Act
            var actual = AssetTool.CreateInputMappingAsset(keepAssetId);

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

        [TestCase(false)]
        [TestCase(true)]
        public void
            CreateInputMappingAsset_ShouldRecreateDefaultInputMappingAssetFileWithTheSameAssetId_WhenInputMappingAssetFileAlreadyExists_GivenKeepAssetId(
                bool keepAssetId)
        {
            // Arrange
            Environment.CurrentDirectory = _temporaryDirectory.Path;

            var originalAssetFilePath = AssetTool.CreateInputMappingAsset();

            var originalAssetData = AssetData.Load(originalAssetFilePath);
            var modifiedAssetData = AssetData.CreateWithJsonContent(originalAssetData.AssetId, originalAssetData.AssetType, new InputMappingAssetContent());
            modifiedAssetData.Save(originalAssetFilePath);

            // Act
            var actualAssetFilePath = AssetTool.CreateInputMappingAsset(keepAssetId);

            // Assert
            var actualAssetData = AssetData.Load(actualAssetFilePath);

            Assert.That(actualAssetFilePath, Is.EqualTo(originalAssetFilePath));
            Assert.That(actualAssetData.AssetId, keepAssetId ? Is.EqualTo(originalAssetData.AssetId) : Is.Not.EqualTo(originalAssetData.AssetId));
            Assert.That(actualAssetData.ReadJsonContent<InputMappingAssetContent>().ActionMappings, Contains.Key("Jump"));
        }

        [Test]
        public void CreateSpriteAnimationAsset_ShouldThrowException_GivenPathToFile()
        {
            // Arrange
            var filePath = Path.Join(_temporaryDirectory.Path, "TestFile.txt");
            File.WriteAllText(filePath, "Test file");

            // Act
            // Assert
            Assert.That(() => AssetTool.CreateSpriteAnimationAsset(filePath), Throws.ArgumentException);
        }

        [Test]
        public void CreateSpriteAnimationAsset_ShouldThrowException_GivenPathToDirectoryWithNoSpriteAssets()
        {
            // Arrange
            var filePath = Path.Join(_temporaryDirectory.Path, "TestFile.txt");
            File.WriteAllText(filePath, "Test file");

            // Act
            // Assert
            Assert.That(() => AssetTool.CreateSpriteAnimationAsset(_temporaryDirectory.Path), Throws.ArgumentException);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void CreateSpriteAnimationAsset_ShouldCreateSpriteAnimationAssetFileInSpecifiedDirectory_GivenPathToDirectoryContainingSpriteAssetFiles(
            bool keepAssetId)
        {
            // Arrange
            CopyAnimationFiles();

            // Act
            var actual = AssetTool.CreateSpriteAnimationAsset(_temporaryDirectory.Path, keepAssetId);

            // Assert
            var spriteAnimationAssetFilePath =
                Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension(new DirectoryInfo(_temporaryDirectory.Path).Name));
            Assert.That(actual, Is.EqualTo(spriteAnimationAssetFilePath));
            Assert.That(File.Exists(spriteAnimationAssetFilePath), Is.True, "Sprite animation asset file was not created.");

            var assetData = AssetData.Load(spriteAnimationAssetFilePath);
            Assert.That(assetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(assetData.AssetType, Is.EqualTo(AnimationAssetTypes.SpriteAnimation));

            var spriteAnimationAssetContent = assetData.ReadJsonContent<SpriteAnimationAssetContent>();
            Assert.That(spriteAnimationAssetContent.DurationTicks, Is.EqualTo(TimeSpan.FromSeconds(1).Ticks));
            Assert.That(spriteAnimationAssetContent.Frames, Has.Length.EqualTo(3));
            Debug.Assert(spriteAnimationAssetContent.Frames != null, "spriteAnimationAssetContent.Frames != null");
            var frame1 = spriteAnimationAssetContent.Frames[0];
            var frame2 = spriteAnimationAssetContent.Frames[1];
            var frame3 = spriteAnimationAssetContent.Frames[2];
            Assert.That(frame1.SpriteAssetId, Is.EqualTo(AssetsIds.TestSpriteAnimationFrame1.Value));
            Assert.That(frame1.Duration, Is.EqualTo(1.0));
            Assert.That(frame2.SpriteAssetId, Is.EqualTo(AssetsIds.TestSpriteAnimationFrame2.Value));
            Assert.That(frame2.Duration, Is.EqualTo(1.0));
            Assert.That(frame3.SpriteAssetId, Is.EqualTo(AssetsIds.TestSpriteAnimationFrame3.Value));
            Assert.That(frame3.Duration, Is.EqualTo(1.0));
        }

        [TestCase(false)]
        [TestCase(true)]
        public void
            CreateSpriteAnimationAsset_ShouldRecreateSpriteAnimationAssetFileWithTheSameAssetId_WhenSpriteAnimationAssetFileAlreadyExists_GivenKeepAssetId(
                bool keepAssetId)
        {
            // Arrange
            CopyAnimationFiles();

            var originalAssetFilePath = AssetTool.CreateSpriteAnimationAsset(_temporaryDirectory.Path);

            var originalAssetData = AssetData.Load(originalAssetFilePath);
            var modifiedAssetData = AssetData.CreateWithJsonContent(originalAssetData.AssetId, originalAssetData.AssetType, new SpriteAnimationAssetContent());
            modifiedAssetData.Save(originalAssetFilePath);

            // Act
            var actualAssetFilePath = AssetTool.CreateSpriteAnimationAsset(_temporaryDirectory.Path, keepAssetId);

            // Assert
            var actualAssetData = AssetData.Load(actualAssetFilePath);

            Assert.That(actualAssetFilePath, Is.EqualTo(originalAssetFilePath));
            Assert.That(actualAssetData.AssetId, keepAssetId ? Is.EqualTo(originalAssetData.AssetId) : Is.Not.EqualTo(originalAssetData.AssetId));
            Assert.That(actualAssetData.ReadJsonContent<SpriteAnimationAssetContent>().DurationTicks, Is.EqualTo(TimeSpan.FromSeconds(1).Ticks));
        }

        private void CopyAnimationFiles()
        {
            File.Copy(GetSourcePath("Texture.png"), GetDestinationPath("Texture.png"));
            File.Copy(GetSourcePath("Texture.geisha-asset"), GetDestinationPath("Texture.geisha-asset"));
            File.Copy(GetSourcePath("Sprite1.sprite.geisha-asset"), GetDestinationPath("Sprite1.sprite.geisha-asset"));
            File.Copy(GetSourcePath("Sprite2.sprite.geisha-asset"), GetDestinationPath("Sprite2.sprite.geisha-asset"));
            File.Copy(GetSourcePath("Sprite3.sprite.geisha-asset"), GetDestinationPath("Sprite3.sprite.geisha-asset"));

            static string GetSourcePath(string fileName) => Utils.GetPathUnderTestDirectory(Path.Combine("Assets", "Animation", fileName));
            string GetDestinationPath(string fileName) => Path.Combine(_temporaryDirectory.Path, fileName);
        }
    }
}