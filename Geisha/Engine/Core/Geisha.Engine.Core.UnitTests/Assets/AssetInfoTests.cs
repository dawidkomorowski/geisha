using System;
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
            var assetId = AssetId.CreateUnique();
            const string assetFilePath = @"Assets\AssetFile";

            // Act
            // Assert
            Assert.That(() => new AssetInfo(assetId, null, assetFilePath), Throws.ArgumentNullException);
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_GivenNullAssetFilePath()
        {
            // Arrange
            var assetId = AssetId.CreateUnique();
            var assetType = typeof(int);

            // Act
            // Assert
            Assert.That(() => new AssetInfo(assetId, assetType, null), Throws.ArgumentNullException);
        }

        [TestCase("345E30DC-5F18-472C-B539-15ECE44B6B60", typeof(object), "some file path",
            "345E30DC-5F18-472C-B539-15ECE44B6B60", typeof(object), "some file path", true)]
        [TestCase("345E30DC-5F18-472C-B539-15ECE44B6B60", typeof(object), "some file path",
            "345E30DC-5F18-472C-B539-15ECE44B6B60", typeof(object), "other file path", false)]
        [TestCase("345E30DC-5F18-472C-B539-15ECE44B6B60", typeof(object), "some file path",
            "345E30DC-5F18-472C-B539-15ECE44B6B60", typeof(int), "some file path", false)]
        [TestCase("345E30DC-5F18-472C-B539-15ECE44B6B60", typeof(object), "some file path",
            "C7CF6FFC-FF65-48D8-BF1B-041E51F8E1C4", typeof(object), "some file path", false)]
        public void EqualityMembersType(string assetIdString1, Type assetType1, string assetFilePath1, string assetIdString2, Type assetType2,
            string assetFilePath2, bool isEqual)
        {
            // Arrange
            var assetId1 = new AssetId(new Guid(assetIdString1));
            var assetId2 = new AssetId(new Guid(assetIdString2));

            var assetInfo1 = new AssetInfo(assetId1, assetType1, assetFilePath1);
            var assetInfo2 = new AssetInfo(assetId2, assetType2, assetFilePath2);

            // Act
            var equals = assetInfo1.Equals(assetInfo2);
            var objectEquals = ((object) assetInfo1).Equals(assetInfo2);
            var getHashCode = assetInfo1.GetHashCode() == assetInfo2.GetHashCode();
            var equalityOperator = assetInfo1 == assetInfo2;
            var inequalityOperator = assetInfo1 != assetInfo2;

            // Assert
            Assert.That(equals, Is.EqualTo(isEqual));
            Assert.That(objectEquals, Is.EqualTo(isEqual));
            Assert.That(getHashCode, Is.EqualTo(isEqual));
            Assert.That(equalityOperator, Is.EqualTo(isEqual));
            Assert.That(inequalityOperator, Is.EqualTo(!isEqual));
        }
    }
}