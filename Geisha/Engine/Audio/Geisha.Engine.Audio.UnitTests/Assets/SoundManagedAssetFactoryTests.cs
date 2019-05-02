﻿using System.Linq;
using Geisha.Common.Serialization;
using Geisha.Engine.Audio.Assets;
using Geisha.Engine.Core.Assets;
using Geisha.Framework.Audio;
using Geisha.Framework.FileSystem;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Audio.UnitTests.Assets
{
    [TestFixture]
    public class SoundManagedAssetFactoryTests
    {
        private IAssetStore _assetStore;
        private IAudioProvider _audioProvider;
        private IFileSystem _fileSystem;
        private IJsonSerializer _jsonSerializer;
        private SoundManagedAssetFactory _spriteManagedAssetFactory;

        [SetUp]
        public void SetUp()
        {
            _assetStore = Substitute.For<IAssetStore>();
            _audioProvider = Substitute.For<IAudioProvider>();
            _fileSystem = Substitute.For<IFileSystem>();
            _jsonSerializer = Substitute.For<IJsonSerializer>();
            _spriteManagedAssetFactory = new SoundManagedAssetFactory(_audioProvider, _fileSystem, _jsonSerializer);
        }

        [Test]
        public void Create_ShouldReturnEmpty_GivenAssetInfoWithNotMatchingAssetType()
        {
            // Arrange
            var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(object), "asset file path");

            // Act
            var actual = _spriteManagedAssetFactory.Create(assetInfo, _assetStore);

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void Create_ShouldReturnSingleAsset_GivenAssetInfoWithMatchingAssetType()
        {
            // Arrange
            var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(ISound), "asset file path");

            // Act
            var actual = _spriteManagedAssetFactory.Create(assetInfo, _assetStore);

            // Assert
            Assert.That(actual, Is.Not.Empty);
            var managedAsset = actual.Single();
            Assert.That(managedAsset, Is.TypeOf<SoundManagedAsset>());
            Assert.That(managedAsset.AssetInfo, Is.EqualTo(assetInfo));
        }
    }
}