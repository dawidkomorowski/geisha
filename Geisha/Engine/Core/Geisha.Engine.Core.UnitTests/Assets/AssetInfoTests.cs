using System;
using Geisha.Engine.Core.Assets;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Assets
{
    [TestFixture]
    public class AssetInfoTests
    {
        [Test]
        public void Constructor_ShouldSetAssetTypeAndAssetIdAndAssetFilePath()
        {
            // Arrange
            var assetType = typeof(object);
            var assetId = Guid.NewGuid();
            var assetFilePath = "some file path";

            // Act
            var actual = new AssetInfo(assetType, assetId, assetFilePath);

            // Assert
            Assert.That(actual.AssetType, Is.EqualTo(assetType));
            Assert.That(actual.AssetId, Is.EqualTo(assetId));
            Assert.That(actual.AssetFilePath, Is.EqualTo(assetFilePath));
        }

        [TestCase(typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path",
            typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path", true)]
        [TestCase(typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path",
            typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "other file path", true)]
        [TestCase(typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path",
            typeof(int), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path", false)]
        [TestCase(typeof(object), "345E30DC-5F18-472C-B539-15ECE44B6B60", "some file path",
            typeof(object), "C7CF6FFC-FF65-48D8-BF1B-041E51F8E1C4", "some file path", false)]
        public void EqualityMembers_ShouldDependOnAssetTypeAndAssetId_ButNotOnAssetFilePath(Type assetType1, string assetId1, string assetFilePath1,
            Type assetType2, string assetId2, string assetFilePath2, bool expected)
        {
            // Arrange
            var assetInfo1 = new AssetInfo(assetType1, new Guid(assetId1), assetFilePath1);
            var assetInfo2 = new AssetInfo(assetType2, new Guid(assetId2), assetFilePath2);

            // Act
            var actual1 = assetInfo1.Equals(assetInfo2);
            var actual2 = assetInfo1.Equals((object) assetInfo2);
            var actual3 = assetInfo1 == assetInfo2;
            var actual4 = assetInfo1 != assetInfo2;
            var actual5 = assetInfo1.GetHashCode() == assetInfo2.GetHashCode();

            // Assert
            Assert.That(actual1, Is.EqualTo(expected));
            Assert.That(actual2, Is.EqualTo(expected));
            Assert.That(actual3, Is.EqualTo(expected));
            Assert.That(actual4, Is.EqualTo(!expected));
            Assert.That(actual5, Is.EqualTo(expected));
        }
    }
}