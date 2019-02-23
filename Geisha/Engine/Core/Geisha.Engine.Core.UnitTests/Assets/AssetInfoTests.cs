﻿using System;
using Geisha.Engine.Core.Assets;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Assets
{
    [TestFixture]
    public class AssetInfoTests
    {
        [Test]
        public void Constructor_ShouldThrowArgumentNullException_GivenNullAssetType()
        {
            // Arrange
            var assetId = new AssetId(Guid.NewGuid());
            const string assetFilePath = @"Assets\AssetFile";

            // Act
            // Assert
            Assert.That(() => new AssetInfo(assetId, null, assetFilePath), Throws.ArgumentNullException);
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_GivenNullAssetFilePath()
        {
            // Arrange
            var assetId = new AssetId(Guid.NewGuid());
            var assetType = typeof(int);

            // Act
            // Assert
            Assert.That(() => new AssetInfo(assetId, assetType, null), Throws.ArgumentNullException);
        }
    }
}