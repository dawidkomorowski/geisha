﻿using System.Linq;
using Geisha.Common.Serialization;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Input.Assets;
using Geisha.Engine.Input.Mapping;
using Geisha.Framework.FileSystem;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Input.UnitTests.Assets
{
    [TestFixture]
    public class InputMappingManagedAssetFactoryTests
    {
        private IAssetStore _assetStore;
        private IFileSystem _fileSystem;
        private IJsonSerializer _jsonSerializer;
        private InputMappingManagedAssetFactory _inputMappingManagedAssetFactory;

        [SetUp]
        public void SetUp()
        {
            _assetStore = Substitute.For<IAssetStore>();
            _fileSystem = Substitute.For<IFileSystem>();
            _jsonSerializer = Substitute.For<IJsonSerializer>();
            _inputMappingManagedAssetFactory = new InputMappingManagedAssetFactory(_fileSystem, _jsonSerializer);
        }

        [Test]
        public void Create_ShouldReturnEmpty_GivenAssetInfoWithNotMatchingAssetType()
        {
            // Arrange
            var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(object), "asset file path");

            // Act
            var actual = _inputMappingManagedAssetFactory.Create(assetInfo, _assetStore);

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void Create_ShouldReturnSingleAsset_GivenAssetInfoWithMatchingAssetType()
        {
            // Arrange
            var assetInfo = new AssetInfo(AssetId.CreateUnique(), typeof(InputMapping), "asset file path");

            // Act
            var actual = _inputMappingManagedAssetFactory.Create(assetInfo, _assetStore);

            // Assert
            Assert.That(actual, Is.Not.Empty);
            var managedAsset = actual.Single();
            Assert.That(managedAsset, Is.TypeOf<InputMappingManagedAsset>());
            Assert.That(managedAsset.AssetInfo, Is.EqualTo(assetInfo));
        }
    }
}