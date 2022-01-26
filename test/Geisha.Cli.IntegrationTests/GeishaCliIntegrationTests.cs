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

namespace Geisha.Cli.IntegrationTests
{
    [TestFixture]
    public class GeishaCliIntegrationTests
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
        public void Asset_Create_Sound_ShouldCreateSoundAssetFileInTheSameDirectoryAsSoundFile_GivenSoundFilePath()
        {
            // Arrange
            var mp3FilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestSound.mp3");
            var mp3FilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestSound.mp3");
            File.Copy(mp3FilePathToCopy, mp3FilePathInTempDir);

            // Act
            RunGeishaCli($"asset create sound \"{mp3FilePathInTempDir}\"");

            // Assert
            var soundAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestSound"));
            Assert.That(File.Exists(soundAssetFilePath), Is.True, "Sound asset file was not created.");

            var assetData = AssetData.Load(soundAssetFilePath);
            Assert.That(assetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(assetData.AssetType, Is.EqualTo(AudioAssetTypes.Sound));

            var soundAssetContent = assetData.ReadJsonContent<SoundAssetContent>();
            Assert.That(soundAssetContent.SoundFilePath, Is.EqualTo("TestSound.mp3"));
        }

        [Test]
        public void Asset_Create_Sound_ShouldRecreateSoundAssetFileWithTheSameAssetId_WhenSoundAssetFileAlreadyExists_GivenKeepAssetId()
        {
            // Arrange
            var mp3FilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestSound.mp3");
            var mp3FilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestSound.mp3");
            File.Copy(mp3FilePathToCopy, mp3FilePathInTempDir);

            var soundAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestSound"));

            RunGeishaCli($"asset create sound \"{mp3FilePathInTempDir}\"");

            var originalAssetData = AssetData.Load(soundAssetFilePath);
            var modifiedAssetData = AssetData.CreateWithJsonContent(originalAssetData.AssetId, originalAssetData.AssetType, new SoundAssetContent());
            modifiedAssetData.Save(soundAssetFilePath);

            // Act
            RunGeishaCli($"asset create sound \"{mp3FilePathInTempDir}\" --keep-asset-id");

            // Assert
            var actualAssetData = AssetData.Load(soundAssetFilePath);

            Assert.That(actualAssetData.AssetId, Is.EqualTo(originalAssetData.AssetId));
            Assert.That(actualAssetData.ReadJsonContent<SoundAssetContent>().SoundFilePath, Is.EqualTo("TestSound.mp3"));
        }

        [Test]
        public void Asset_Create_Texture_ShouldCreateTextureAssetFileInTheSameDirectoryAsTextureFile_GivenTextureFilePath()
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            // Act
            RunGeishaCli($"asset create texture \"{pngFilePathInTempDir}\"");

            // Assert
            var textureAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestTexture"));
            Assert.That(File.Exists(textureAssetFilePath), Is.True, "Texture asset file was not created.");

            var assetData = AssetData.Load(textureAssetFilePath);
            Assert.That(assetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(assetData.AssetType, Is.EqualTo(RenderingAssetTypes.Texture));

            var textureAssetContent = assetData.ReadJsonContent<TextureAssetContent>();
            Assert.That(textureAssetContent.TextureFilePath, Is.EqualTo("TestTexture.png"));
        }

        [Test]
        public void Asset_Create_Texture_ShouldRecreateTextureAssetFileWithTheSameAssetId_WhenTextureAssetFileAlreadyExists_GivenKeepAssetId()
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            var textureAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestTexture"));

            RunGeishaCli($"asset create texture \"{pngFilePathInTempDir}\"");

            var originalAssetData = AssetData.Load(textureAssetFilePath);
            var modifiedAssetData = AssetData.CreateWithJsonContent(originalAssetData.AssetId, originalAssetData.AssetType, new TextureAssetContent());
            modifiedAssetData.Save(textureAssetFilePath);

            // Act
            RunGeishaCli($"asset create texture \"{pngFilePathInTempDir}\" --keep-asset-id");

            // Assert
            var actualAssetData = AssetData.Load(textureAssetFilePath);

            Assert.That(actualAssetData.AssetId, Is.EqualTo(originalAssetData.AssetId));
            Assert.That(actualAssetData.ReadJsonContent<TextureAssetContent>().TextureFilePath, Is.EqualTo("TestTexture.png"));
        }

        [Test]
        public void Asset_Create_Sprite_ShouldCreateSpriteAssetFileInTheSameDirectoryAsTextureAssetFile_GivenTextureAssetFilePath()
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            var textureAssetFilePathToCopy = Utils.GetPathUnderTestDirectory(AssetFileUtils.AppendExtension(@"Assets\TestTexture"));
            var textureAssetFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestTexture"));
            File.Copy(textureAssetFilePathToCopy, textureAssetFilePathInTempDir);

            // Act
            RunGeishaCli($"asset create sprite \"{textureAssetFilePathInTempDir}\"");

            // Assert
            var spriteAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestTexture.sprite"));
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

        [Test]
        public void
            Asset_Create_Sprite_ShouldRecreateSpriteAssetFileWithTheSameAssetId_WhenSpriteAssetFileAlreadyExists_GivenTextureAssetFilePath_And_KeepAssetId()
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            var textureAssetFilePathToCopy = Utils.GetPathUnderTestDirectory(AssetFileUtils.AppendExtension(@"Assets\TestTexture"));
            var textureAssetFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestTexture"));
            File.Copy(textureAssetFilePathToCopy, textureAssetFilePathInTempDir);

            var spriteAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestTexture.sprite"));

            RunGeishaCli($"asset create sprite \"{textureAssetFilePathInTempDir}\"");

            var originalAssetData = AssetData.Load(spriteAssetFilePath);
            var modifiedAssetData = AssetData.CreateWithJsonContent(originalAssetData.AssetId, originalAssetData.AssetType, new SpriteAssetContent());
            modifiedAssetData.Save(spriteAssetFilePath);

            // Act
            RunGeishaCli($"asset create sprite \"{textureAssetFilePathInTempDir}\" --keep-asset-id");

            // Assert
            var actualAssetData = AssetData.Load(spriteAssetFilePath);

            Assert.That(actualAssetData.AssetId, Is.EqualTo(originalAssetData.AssetId));
            Assert.That(actualAssetData.ReadJsonContent<SpriteAssetContent>().TextureAssetId, Is.EqualTo(AssetsIds.TestTexture.Value));
        }

        [Test]
        public void Asset_Create_Sprite_ShouldCreateTextureAssetFileAndSpriteAssetFileInTheSameFolderAsTextureFile_GivenTextureFilePath()
        {
            // Arrange
            var pngFilePathToCopy = Utils.GetPathUnderTestDirectory(@"Assets\TestTexture.png");
            var pngFilePathInTempDir = Path.Combine(_temporaryDirectory.Path, "TestTexture.png");
            File.Copy(pngFilePathToCopy, pngFilePathInTempDir);

            // Act
            RunGeishaCli($"asset create sprite \"{pngFilePathInTempDir}\"");

            // Assert
            var textureAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestTexture"));
            Assert.That(File.Exists(textureAssetFilePath), Is.True, "Texture asset file was not created.");

            var textureAssetData = AssetData.Load(textureAssetFilePath);
            Assert.That(textureAssetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(textureAssetData.AssetType, Is.EqualTo(RenderingAssetTypes.Texture));
            var textureAssetContent = textureAssetData.ReadJsonContent<TextureAssetContent>();
            Assert.That(textureAssetContent.TextureFilePath, Is.EqualTo("TestTexture.png"));

            var spriteAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("TestTexture.sprite"));
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

        [Test]
        public void Asset_Create_InputMapping_ShouldCreateDefaultInputMappingAssetFileInCurrentDirectory()
        {
            // Arrange
            // Act
            RunGeishaCli("asset create input-mapping", _temporaryDirectory.Path);

            // Assert
            var inputMappingAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("DefaultInputMapping"));
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

        [Test]
        public void
            Asset_Create_InputMapping_ShouldRecreateDefaultInputMappingAssetFileWithTheSameAssetId_WhenInputMappingAssetFileAlreadyExists_GivenKeepAssetId()
        {
            // Arrange
            var inputMappingAssetFilePath = Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension("DefaultInputMapping"));

            RunGeishaCli("asset create input-mapping", _temporaryDirectory.Path);

            var originalAssetData = AssetData.Load(inputMappingAssetFilePath);
            var modifiedAssetData = AssetData.CreateWithJsonContent(originalAssetData.AssetId, originalAssetData.AssetType, new InputMappingAssetContent());
            modifiedAssetData.Save(inputMappingAssetFilePath);

            // Act
            RunGeishaCli("asset create input-mapping --keep-asset-id", _temporaryDirectory.Path);

            // Assert
            var actualAssetData = AssetData.Load(inputMappingAssetFilePath);

            Assert.That(actualAssetData.AssetId, Is.EqualTo(originalAssetData.AssetId));
            Assert.That(actualAssetData.ReadJsonContent<InputMappingAssetContent>().ActionMappings, Contains.Key("Jump"));
        }

        [Test]
        public void Asset_Create_SpriteAnimation_ShouldCreateSpriteAnimationAssetFileInSpecifiedDirectory_GivenPathToDirectoryContainingSpriteAssetFiles()
        {
            // Arrange
            CopyAnimationFiles();

            // Act
            RunGeishaCli($"asset create sprite-animation \"{_temporaryDirectory.Path}\"");

            // Assert
            var spriteAnimationAssetFilePath =
                Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension(new DirectoryInfo(_temporaryDirectory.Path).Name));
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
        public void
            Asset_Create_SpriteAnimation_ShouldRecreateSpriteAnimationAssetFileWithTheSameAssetId_WhenSpriteAnimationAssetFileAlreadyExists_GivenKeepAssetId()
        {
            // Arrange
            CopyAnimationFiles();

            var spriteAnimationAssetFilePath =
                Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension(new DirectoryInfo(_temporaryDirectory.Path).Name));

            RunGeishaCli($"asset create sprite-animation \"{_temporaryDirectory.Path}\"");

            var originalAssetData = AssetData.Load(spriteAnimationAssetFilePath);
            var modifiedAssetData = AssetData.CreateWithJsonContent(originalAssetData.AssetId, originalAssetData.AssetType, new SpriteAnimationAssetContent());
            modifiedAssetData.Save(spriteAnimationAssetFilePath);

            // Act
            RunGeishaCli($"asset create sprite-animation \"{_temporaryDirectory.Path}\" --keep-asset-id");

            // Assert
            var actualAssetData = AssetData.Load(spriteAnimationAssetFilePath);

            Assert.That(actualAssetData.AssetId, Is.EqualTo(originalAssetData.AssetId));
            Assert.That(actualAssetData.ReadJsonContent<SpriteAnimationAssetContent>().DurationTicks, Is.EqualTo(TimeSpan.FromSeconds(1).Ticks));
        }

        [Test]
        public void Asset_Create_SpriteAnimation_ShouldCreateSpriteAnimationAssetFileInSpecifiedDirectory_GivenPathToDirectoryAndFilePattern()
        {
            // Arrange
            CopyAnimationFiles();
            CreateAdditionalAnimationFiles();

            // Act
            RunGeishaCli($"asset create sprite-animation \"{_temporaryDirectory.Path}\" --file-pattern Sprite*");

            // Assert
            var spriteAnimationAssetFilePath =
                Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension(new DirectoryInfo(_temporaryDirectory.Path).Name));
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
        public void Asset_Create_SpriteAnimation_ShouldCreateSpriteAnimationAssetFileInSpecifiedDirectory_GivenPathToDirectoryAndFiles()
        {
            // Arrange
            CopyAnimationFiles();

            // Act
            RunGeishaCli(
                $"asset create sprite-animation \"{_temporaryDirectory.Path}\" --files \"{Path.Combine(_temporaryDirectory.Path, "Sprite3.sprite.geisha-asset")}\" \"{Path.Combine(_temporaryDirectory.Path, "Sprite1.sprite.geisha-asset")}\"");

            // Assert
            var spriteAnimationAssetFilePath =
                Path.Combine(_temporaryDirectory.Path, AssetFileUtils.AppendExtension(new DirectoryInfo(_temporaryDirectory.Path).Name));
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

        private static void RunGeishaCli(string arguments, string? workingDirectory = null)
        {
            var processStartInfo = new ProcessStartInfo("Geisha.Cli.exe", arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            if (workingDirectory != null)
            {
                processStartInfo.WorkingDirectory = workingDirectory;
            }

            var geishaCli = Process.Start(processStartInfo) ?? throw new InvalidOperationException("Process could not be started.");

            var standardError = geishaCli.StandardError.ReadToEnd();

            Console.WriteLine("------------------------------");
            Console.WriteLine("   Geisha CLI output start");
            Console.WriteLine("------------------------------");
            Console.WriteLine(standardError);
            Console.WriteLine(geishaCli.StandardOutput.ReadToEnd());
            Console.WriteLine("------------------------------");
            Console.WriteLine("    Geisha CLI output end");
            Console.WriteLine("------------------------------");

            if (standardError != string.Empty)
            {
                Assert.Fail(standardError);
            }
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
    }
}