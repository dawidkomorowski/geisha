using System;
using System.Diagnostics;
using System.IO;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Assets.Serialization;
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

        private static void RunGeishaCli(string arguments)
        {
            var processStartInfo = new ProcessStartInfo("Geisha.Cli.exe", arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var geishaCli = Process.Start(processStartInfo) ?? throw new InvalidOperationException("Process could not be started.");

            Console.WriteLine("------------------------------");
            Console.WriteLine("   Geisha CLI output start");
            Console.WriteLine("------------------------------");
            Console.WriteLine(geishaCli.StandardError.ReadToEnd());
            Console.WriteLine(geishaCli.StandardOutput.ReadToEnd());
            Console.WriteLine("------------------------------");
            Console.WriteLine("    Geisha CLI output end");
            Console.WriteLine("------------------------------");
        }
    }
}