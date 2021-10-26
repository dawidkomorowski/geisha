using System;
using System.Diagnostics;
using System.IO;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Audio.Assets.Serialization;
using Geisha.Engine.Core.Assets;
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

            using var fileStream = File.OpenRead(soundAssetFilePath);
            var assetData = AssetData.Load(fileStream);
            Assert.That(assetData.AssetId, Is.Not.EqualTo(Guid.Empty));
            Assert.That(assetData.AssetType, Is.EqualTo(AudioAssetTypes.Sound));

            var soundAssetContent = assetData.ReadJsonContent<SoundAssetContent>();
            Assert.That(soundAssetContent.SoundFilePath, Is.EqualTo("TestSound.mp3"));
        }

        private static void RunGeishaCli(string arguments)
        {
            var processStartInfo = new ProcessStartInfo("Geisha.Cli.exe", arguments)
            {
                RedirectStandardOutput = true
            };

            var geishaCli = Process.Start(processStartInfo) ?? throw new InvalidOperationException("Process could not be started.");

            Console.WriteLine("------------------------------");
            Console.WriteLine("   Geisha CLI output start");
            Console.WriteLine("------------------------------");
            Console.WriteLine(geishaCli.StandardOutput.ReadToEnd());
            Console.WriteLine("------------------------------");
            Console.WriteLine("    Geisha CLI output end");
            Console.WriteLine("------------------------------");
        }
    }
}