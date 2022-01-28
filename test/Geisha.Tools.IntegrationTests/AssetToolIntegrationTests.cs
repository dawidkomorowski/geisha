using System;
using System.Diagnostics;
using System.IO;
using Geisha.Common.Math.Serialization;
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

        #region CreateSoundAsset

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

        #endregion

        #region CreateTextureAsset

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

        #endregion

        #region CanCreateSpriteAssetFromFile

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

        #endregion

        #region CreateSpriteAsset

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
        public void CreateSpriteAsset_ShouldThrowException_GivenCountLessThan_1()
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            var textureAssetFilePathToCopy = Utils.GetPathUnderTestDirectory(AssetFileUtils.AppendExtension(@"Assets\TestTexture"));
            var textureAssetFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestTexture"));
            File.Copy(textureAssetFilePathToCopy, textureAssetFilePathInTempDir);

            // Act
            // Assert
            Assert.That(() => AssetTool.CreateSpriteAsset(textureAssetFilePathInTempDir, count: 0), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void CreateSpriteAsset_ShouldThrowException_GivenParametersExceedingTextureDimensions()
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\SpriteSheet\TestSpriteSheet.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestSpriteSheet.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            var textureAssetFilePathToCopy = Utils.GetPathUnderTestDirectory(AssetFileUtils.AppendExtension(@"Assets\SpriteSheet\TestSpriteSheet"));
            var textureAssetFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestSpriteSheet"));
            File.Copy(textureAssetFilePathToCopy, textureAssetFilePathInTempDir);

            // Act
            // Assert
            Assert.That(() => AssetTool.CreateSpriteAsset(textureAssetFilePathInTempDir, width: 50, height: 50, count: 16),
                Throws.InvalidOperationException);
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
            Assert.That(actual.spriteAssetFilePaths, Has.Length.EqualTo(1));
            Assert.That(actual.spriteAssetFilePaths[0], Is.EqualTo(spriteAssetFilePath));
            Assert.That(actual.textureAssetFilePath, Is.Null);
            Assert.That(File.Exists(spriteAssetFilePath), Is.True, "Sprite asset file was not created.");

            var assetData = AssetData.Load(spriteAssetFilePath);
            Assert.That(assetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(assetData.AssetType, Is.EqualTo(RenderingAssetTypes.Sprite));
            var spriteAssetContent = assetData.ReadJsonContent<SpriteAssetContent>();
            Assert.That(spriteAssetContent.TextureAssetId, Is.EqualTo(AssetsIds.TestTexture.Value));
            Assert.That(spriteAssetContent.SourceUV.X, Is.Zero);
            Assert.That(spriteAssetContent.SourceUV.Y, Is.Zero);
            Assert.That(spriteAssetContent.SourceDimensions.X, Is.EqualTo(10));
            Assert.That(spriteAssetContent.SourceDimensions.Y, Is.EqualTo(10));
            Assert.That(spriteAssetContent.Pivot.X, Is.EqualTo(5));
            Assert.That(spriteAssetContent.Pivot.Y, Is.EqualTo(5));
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

            var originalAssetFilePath = AssetTool.CreateSpriteAsset(textureAssetFilePathInTempDir).spriteAssetFilePaths[0];

            var originalAssetData = AssetData.Load(originalAssetFilePath);
            var modifiedAssetData = AssetData.CreateWithJsonContent(originalAssetData.AssetId, originalAssetData.AssetType, new SpriteAssetContent());
            modifiedAssetData.Save(originalAssetFilePath);

            // Act
            var actualAssetFilePaths = AssetTool.CreateSpriteAsset(textureAssetFilePathInTempDir, keepAssetId).spriteAssetFilePaths;

            // Assert
            Assert.That(actualAssetFilePaths, Has.Length.EqualTo(1));
            var actualAssetFilePath = actualAssetFilePaths[0];
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
            Assert.That(actual.spriteAssetFilePaths, Has.Length.EqualTo(1));
            Assert.That(actual.spriteAssetFilePaths[0], Is.EqualTo(spriteAssetFilePath));
            Assert.That(File.Exists(spriteAssetFilePath), Is.True, "Sprite asset file was not created.");

            var spriteAssetData = AssetData.Load(spriteAssetFilePath);
            Assert.That(spriteAssetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(spriteAssetData.AssetType, Is.EqualTo(RenderingAssetTypes.Sprite));
            var spriteAssetContent = spriteAssetData.ReadJsonContent<SpriteAssetContent>();
            Assert.That(spriteAssetContent.TextureAssetId, Is.EqualTo(textureAssetData.AssetId.Value));
            Assert.That(spriteAssetContent.SourceUV.X, Is.Zero);
            Assert.That(spriteAssetContent.SourceUV.Y, Is.Zero);
            Assert.That(spriteAssetContent.SourceDimensions.X, Is.EqualTo(10));
            Assert.That(spriteAssetContent.SourceDimensions.Y, Is.EqualTo(10));
            Assert.That(spriteAssetContent.Pivot.X, Is.EqualTo(5));
            Assert.That(spriteAssetContent.Pivot.Y, Is.EqualTo(5));
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

            var (originalSpriteAssetFilePaths, originalTextureAssetFilePath) = AssetTool.CreateSpriteAsset(pngFilePathInTempDir);
            var originalSpriteAssetFilePath = originalSpriteAssetFilePaths[0];

            var originalTextureAssetData = AssetData.Load(originalTextureAssetFilePath);
            var modifiedTextureAssetData =
                AssetData.CreateWithJsonContent(originalTextureAssetData.AssetId, originalTextureAssetData.AssetType, new TextureAssetContent());
            modifiedTextureAssetData.Save(originalTextureAssetFilePath);

            var originalSpriteAssetData = AssetData.Load(originalSpriteAssetFilePath);
            var modifiedSpriteAssetData =
                AssetData.CreateWithJsonContent(originalSpriteAssetData.AssetId, originalSpriteAssetData.AssetType, new SpriteAssetContent());
            modifiedSpriteAssetData.Save(originalSpriteAssetFilePath);

            // Act
            var (actualSpriteAssetFilePaths, actualTextureAssetFilePath) = AssetTool.CreateSpriteAsset(pngFilePathInTempDir, keepAssetId);

            // Assert
            Assert.That(actualSpriteAssetFilePaths, Has.Length.EqualTo(1));
            var actualSpriteAssetFilePath = actualSpriteAssetFilePaths[0];
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

        #region CreateSpriteAssetTestCases

        public sealed class SpriteAssetTestCase
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }
            public int Count { get; set; }

            public ExpectedData[] Expected { get; set; }

            public sealed class ExpectedData
            {
                public string FileName { get; set; }
                public SpriteAssetContent SpriteAssetContent { get; set; }
            }

            public override string ToString() =>
                $"{nameof(X)}: {X}, {nameof(Y)}: {Y}, {nameof(Width)}: {Width}, {nameof(Height)}: {Height}, {nameof(Count)}: {Count}";
        }

        public static SpriteAssetTestCase[] CreateSpriteAssetTestCases => new[]
        {
            new SpriteAssetTestCase
            {
                X = 0, Y = 0, Width = 0, Height = 0, Count = 1, Expected = new[]
                {
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 0, Y = 0 },
                            SourceDimensions = new SerializableVector2 { X = 100, Y = 100 },
                            Pivot = new SerializableVector2 { X = 50, Y = 50 },
                            PixelsPerUnit = 1
                        }
                    }
                }
            },
            new SpriteAssetTestCase
            {
                X = 50, Y = 0, Width = 0, Height = 0, Count = 1, Expected = new[]
                {
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 50, Y = 0 },
                            SourceDimensions = new SerializableVector2 { X = 50, Y = 100 },
                            Pivot = new SerializableVector2 { X = 25, Y = 50 },
                            PixelsPerUnit = 1
                        }
                    }
                }
            },
            new SpriteAssetTestCase
            {
                X = 0, Y = 50, Width = 0, Height = 0, Count = 1, Expected = new[]
                {
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 0, Y = 50 },
                            SourceDimensions = new SerializableVector2 { X = 100, Y = 50 },
                            Pivot = new SerializableVector2 { X = 50, Y = 25 },
                            PixelsPerUnit = 1
                        }
                    }
                }
            },
            new SpriteAssetTestCase
            {
                X = 50, Y = 50, Width = 0, Height = 0, Count = 1, Expected = new[]
                {
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 50, Y = 50 },
                            SourceDimensions = new SerializableVector2 { X = 50, Y = 50 },
                            Pivot = new SerializableVector2 { X = 25, Y = 25 },
                            PixelsPerUnit = 1
                        }
                    }
                }
            },
            new SpriteAssetTestCase
            {
                X = 50, Y = 50, Width = 20, Height = 0, Count = 1, Expected = new[]
                {
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 50, Y = 50 },
                            SourceDimensions = new SerializableVector2 { X = 20, Y = 50 },
                            Pivot = new SerializableVector2 { X = 10, Y = 25 },
                            PixelsPerUnit = 1
                        }
                    }
                }
            },
            new SpriteAssetTestCase
            {
                X = 50, Y = 50, Width = 0, Height = 20, Count = 1, Expected = new[]
                {
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 50, Y = 50 },
                            SourceDimensions = new SerializableVector2 { X = 50, Y = 20 },
                            Pivot = new SerializableVector2 { X = 25, Y = 10 },
                            PixelsPerUnit = 1
                        }
                    }
                }
            },
            new SpriteAssetTestCase
            {
                X = 50, Y = 50, Width = 20, Height = 20, Count = 1, Expected = new[]
                {
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 50, Y = 50 },
                            SourceDimensions = new SerializableVector2 { X = 20, Y = 20 },
                            Pivot = new SerializableVector2 { X = 10, Y = 10 },
                            PixelsPerUnit = 1
                        }
                    }
                }
            },
            new SpriteAssetTestCase
            {
                X = 0, Y = 0, Width = 25, Height = 25, Count = 2, Expected = new[]
                {
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_0.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 0, Y = 0 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_1.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 25, Y = 0 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    }
                }
            },
            new SpriteAssetTestCase
            {
                X = 0, Y = 0, Width = 25, Height = 25, Count = 4, Expected = new[]
                {
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_0.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 0, Y = 0 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_1.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 25, Y = 0 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_2.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 50, Y = 0 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_3.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 75, Y = 0 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    }
                }
            },
            new SpriteAssetTestCase
            {
                X = 30, Y = 40, Width = 20, Height = 10, Count = 2, Expected = new[]
                {
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_0.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 30, Y = 40 },
                            SourceDimensions = new SerializableVector2 { X = 20, Y = 10 },
                            Pivot = new SerializableVector2 { X = 10, Y = 5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_1.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 50, Y = 40 },
                            SourceDimensions = new SerializableVector2 { X = 20, Y = 10 },
                            Pivot = new SerializableVector2 { X = 10, Y = 5 },
                            PixelsPerUnit = 1
                        }
                    }
                }
            },
            new SpriteAssetTestCase
            {
                X = 60, Y = 20, Width = 30, Height = 20, Count = 2, Expected = new[]
                {
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_0.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 60, Y = 20 },
                            SourceDimensions = new SerializableVector2 { X = 30, Y = 20 },
                            Pivot = new SerializableVector2 { X = 15, Y = 10 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_1.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 0, Y = 40 },
                            SourceDimensions = new SerializableVector2 { X = 30, Y = 20 },
                            Pivot = new SerializableVector2 { X = 15, Y = 10 },
                            PixelsPerUnit = 1
                        }
                    }
                }
            },
            new SpriteAssetTestCase
            {
                X = 0, Y = 0, Width = 25, Height = 25, Count = 16, Expected = new[]
                {
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_00.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 0, Y = 0 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_01.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 25, Y = 0 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_02.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 50, Y = 0 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_03.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 75, Y = 0 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_04.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 0, Y = 25 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_05.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 25, Y = 25 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_06.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 50, Y = 25 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_07.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 75, Y = 25 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_08.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 0, Y = 50 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_09.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 25, Y = 50 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_10.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 50, Y = 50 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_11.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 75, Y = 50 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_12.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 0, Y = 75 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_13.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 25, Y = 75 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_14.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 50, Y = 75 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    },
                    new SpriteAssetTestCase.ExpectedData
                    {
                        FileName = "TestSpriteSheet_15.sprite",
                        SpriteAssetContent = new SpriteAssetContent
                        {
                            TextureAssetId = AssetsIds.TestSpriteSheetTexture.Value,
                            SourceUV = new SerializableVector2 { X = 75, Y = 75 },
                            SourceDimensions = new SerializableVector2 { X = 25, Y = 25 },
                            Pivot = new SerializableVector2 { X = 12.5, Y = 12.5 },
                            PixelsPerUnit = 1
                        }
                    }
                }
            }
        };

        #endregion

        [TestCaseSource(nameof(CreateSpriteAssetTestCases))]
        public void CreateSpriteAsset_ShouldCreateSpriteAssetFiles_GivenSpriteParameters(SpriteAssetTestCase testCase)
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\SpriteSheet\TestSpriteSheet.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestSpriteSheet.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            var textureAssetFilePathToCopy = Utils.GetPathUnderTestDirectory(AssetFileUtils.AppendExtension(@"Assets\SpriteSheet\TestSpriteSheet"));
            var textureAssetFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestSpriteSheet"));
            File.Copy(textureAssetFilePathToCopy, textureAssetFilePathInTempDir);

            // Act
            var actual = AssetTool.CreateSpriteAsset(textureAssetFilePathInTempDir,
                x: testCase.X,
                y: testCase.Y,
                width: testCase.Width,
                height: testCase.Height,
                count: testCase.Count);

            // Assert
            Assert.That(actual.textureAssetFilePath, Is.Null);
            Assert.That(actual.spriteAssetFilePaths, Has.Length.EqualTo(testCase.Expected.Length));

            for (var i = 0; i < actual.spriteAssetFilePaths.Length; i++)
            {
                var expected = testCase.Expected[i];
                var spriteAssetFilePath = actual.spriteAssetFilePaths[i];

                var expectedAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension(expected.FileName));
                Assert.That(spriteAssetFilePath, Is.EqualTo(expectedAssetFilePath));

                Assert.That(File.Exists(spriteAssetFilePath), Is.True, "Sprite asset file was not created.");

                var assetData = AssetData.Load(spriteAssetFilePath);
                Assert.That(assetData.AssetId, Is.Not.EqualTo(Guid.Empty));
                Assert.That(assetData.AssetType, Is.EqualTo(RenderingAssetTypes.Sprite));
                var spriteAssetContent = assetData.ReadJsonContent<SpriteAssetContent>();
                Assert.That(spriteAssetContent.TextureAssetId, Is.EqualTo(expected.SpriteAssetContent.TextureAssetId));
                Assert.That(spriteAssetContent.SourceUV.X, Is.EqualTo(expected.SpriteAssetContent.SourceUV.X));
                Assert.That(spriteAssetContent.SourceUV.Y, Is.EqualTo(expected.SpriteAssetContent.SourceUV.Y));
                Assert.That(spriteAssetContent.SourceDimensions.X, Is.EqualTo(expected.SpriteAssetContent.SourceDimensions.X));
                Assert.That(spriteAssetContent.SourceDimensions.Y, Is.EqualTo(expected.SpriteAssetContent.SourceDimensions.Y));
                Assert.That(spriteAssetContent.Pivot.X, Is.EqualTo(expected.SpriteAssetContent.Pivot.X));
                Assert.That(spriteAssetContent.Pivot.Y, Is.EqualTo(expected.SpriteAssetContent.Pivot.Y));
                Assert.That(spriteAssetContent.PixelsPerUnit, Is.EqualTo(expected.SpriteAssetContent.PixelsPerUnit));
            }
        }

        #endregion

        #region CreateInputMappingAsset

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

        #endregion

        #region CreateSpriteAnimationAsset

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

        [Test]
        public void CreateSpriteAnimationAsset_ShouldThrowException_GivenFilePatternAndFilesPaths()
        {
            // Arrange
            CopyAnimationFiles();

            // Act
            // Assert
            Assert.That(
                () => AssetTool.CreateSpriteAnimationAsset(_temporaryDirectory.Path, filePattern: "Sprite*",
                    filesPaths: new[] { Path.Combine(_temporaryDirectory.Path, "Sprite1.sprite.geisha-asset") }),
                Throws.ArgumentException);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void CreateSpriteAnimationAsset_ShouldCreateSpriteAnimationAssetFileInSpecifiedDirectory_GivenPathToDirectoryContainingSpriteAssetFiles(
            bool keepAssetId)
        {
            // Arrange
            CopyAnimationFiles();

            // Act
            var actual = AssetTool.CreateSpriteAnimationAsset(_temporaryDirectory.Path, keepAssetId: keepAssetId);

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
            var actualAssetFilePath = AssetTool.CreateSpriteAnimationAsset(_temporaryDirectory.Path, keepAssetId: keepAssetId);

            // Assert
            var actualAssetData = AssetData.Load(actualAssetFilePath);

            Assert.That(actualAssetFilePath, Is.EqualTo(originalAssetFilePath));
            Assert.That(actualAssetData.AssetId, keepAssetId ? Is.EqualTo(originalAssetData.AssetId) : Is.Not.EqualTo(originalAssetData.AssetId));
            Assert.That(actualAssetData.ReadJsonContent<SpriteAnimationAssetContent>().DurationTicks, Is.EqualTo(TimeSpan.FromSeconds(1).Ticks));
        }

        [Test]
        public void CreateSpriteAnimationAsset_ShouldCreateSpriteAnimationAssetFileInSpecifiedDirectory_GivenPathToDirectoryAndFilePattern()
        {
            // Arrange
            CopyAnimationFiles();
            CreateAdditionalAnimationFiles();

            // Act
            var actual = AssetTool.CreateSpriteAnimationAsset(_temporaryDirectory.Path, filePattern: "Sprite*");

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

        [Test]
        public void CreateSpriteAnimationAsset_ShouldCreateSpriteAnimationAssetFileInSpecifiedDirectory_GivenPathToDirectoryAndFilesPaths()
        {
            // Arrange
            CopyAnimationFiles();

            // Act
            var actual = AssetTool.CreateSpriteAnimationAsset(_temporaryDirectory.Path, filesPaths: new[]
            {
                Path.Combine(_temporaryDirectory.Path, "Sprite3.sprite.geisha-asset"),
                Path.Combine(_temporaryDirectory.Path, "Sprite1.sprite.geisha-asset")
            });

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
            Assert.That(spriteAnimationAssetContent.Frames, Has.Length.EqualTo(2));
            Debug.Assert(spriteAnimationAssetContent.Frames != null, "spriteAnimationAssetContent.Frames != null");
            var frame1 = spriteAnimationAssetContent.Frames[0];
            var frame2 = spriteAnimationAssetContent.Frames[1];
            Assert.That(frame1.SpriteAssetId, Is.EqualTo(AssetsIds.TestSpriteAnimationFrame3.Value));
            Assert.That(frame1.Duration, Is.EqualTo(1.0));
            Assert.That(frame2.SpriteAssetId, Is.EqualTo(AssetsIds.TestSpriteAnimationFrame1.Value));
            Assert.That(frame2.Duration, Is.EqualTo(1.0));
        }

        #endregion

        #region Helpers

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

        private void CreateAdditionalAnimationFiles()
        {
            CopySpriteWithNewGuid("Sprite1.sprite.geisha-asset");
            CopySpriteWithNewGuid("Sprite2.sprite.geisha-asset");
            CopySpriteWithNewGuid("Sprite3.sprite.geisha-asset");

            void CopySpriteWithNewGuid(string fileName)
            {
                var originalAssetData = AssetData.Load(Path.Combine(_temporaryDirectory.Path, fileName));
                var modifiedAssetData = AssetData.CreateWithJsonContent(AssetId.CreateUnique(), originalAssetData.AssetType,
                    originalAssetData.ReadJsonContent<SpriteAnimationAssetContent>());
                modifiedAssetData.Save(Path.Combine(_temporaryDirectory.Path, $"Additional{fileName}"));
            }
        }

        #endregion
    }
}