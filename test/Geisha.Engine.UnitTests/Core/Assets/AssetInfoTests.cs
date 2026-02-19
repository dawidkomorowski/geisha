using System;
using Geisha.Engine.Core.Assets;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Assets
{
    [TestFixture]
    public class AssetInfoTests
    {
        [Test]
        public void Constructor_ShouldCreateDefaultAssetInfo_GivenNoParameters()
        {
            // Arrange
            // Act
            var actual = new AssetInfo();

            // Assert
            Assert.That(actual.AssetId, Is.EqualTo(default(AssetId)));
            Assert.That(actual.AssetType, Is.EqualTo(default(AssetType)));
            Assert.That(actual.AssetFilePath, Is.Empty);
        }

        [Test]
        public void Constructor_ShouldThrowArgumentNullException_GivenNullAssetFilePath()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => new AssetInfo(default, default, null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Constructor_ShouldCreateAssetInfo_GivenAllParameters()
        {
            // Arrange
            var assetId = AssetId.CreateUnique();
            var assetType = new AssetType("Asset Type");
            const string assetFilePath = "some file path";

            // Act
            var actual = new AssetInfo(assetId, assetType, assetFilePath);

            // Assert
            Assert.That(actual.AssetId, Is.EqualTo(assetId));
            Assert.That(actual.AssetType, Is.EqualTo(assetType));
            Assert.That(actual.AssetFilePath, Is.EqualTo(assetFilePath));
        }

        [Test]
        public void ToString_ShouldReturn_AssetId_AssetType_AssetFilePath()
        {
            // Arrange
            var assetId = AssetId.Parse("E0598EF9-A13E-4915-8DE3-AD5FB7036EF5");
            var assetType = new AssetType("Asset Type");
            const string assetFilePath = "some file path";

            var assetInfo = new AssetInfo(assetId, assetType, assetFilePath);

            // Act
            var actual = assetInfo.ToString();

            // Assert
            Assert.That(actual,
                Is.EqualTo("AssetId: AssetId { Value = e0598ef9-a13e-4915-8de3-ad5fb7036ef5 }, AssetType: Asset Type, AssetFilePath: some file path"));
        }

        [TestCase("345E30DC-5F18-472C-B539-15ECE44B6B60", "Asset Type 1", "some file path",
            "345E30DC-5F18-472C-B539-15ECE44B6B60", "Asset Type 1", "some file path", true)]
        [TestCase("345E30DC-5F18-472C-B539-15ECE44B6B60", "Asset Type 1", "some file path",
            "345E30DC-5F18-472C-B539-15ECE44B6B60", "Asset Type 1", "other file path", false)]
        [TestCase("345E30DC-5F18-472C-B539-15ECE44B6B60", "Asset Type 1", "some file path",
            "345E30DC-5F18-472C-B539-15ECE44B6B60", "Asset Type 2", "some file path", false)]
        [TestCase("345E30DC-5F18-472C-B539-15ECE44B6B60", "Asset Type 1", "some file path",
            "C7CF6FFC-FF65-48D8-BF1B-041E51F8E1C4", "Asset Type 1", "some file path", false)]
        public void EqualityMembers_ShouldEqualAssetInfo_When_AssetId_And_AssetType_And_AssetFilePath_IsEqual(string assetIdString1, string assetTypeString1,
            string assetFilePath1, string assetIdString2, string assetTypeString2, string assetFilePath2, bool expectedIsEqual)
        {
            // Arrange
            var assetId1 = AssetId.Parse(assetIdString1);
            var assetId2 = AssetId.Parse(assetIdString2);

            var assetType1 = new AssetType(assetTypeString1);
            var assetType2 = new AssetType(assetTypeString2);

            var assetInfo1 = new AssetInfo(assetId1, assetType1, assetFilePath1);
            var assetInfo2 = new AssetInfo(assetId2, assetType2, assetFilePath2);

            // Act
            // Assert
            AssertEqualityMembers
                .ForValues(assetInfo1, assetInfo2)
                .UsingEqualityOperator((x, y) => x == y)
                .UsingInequalityOperator((x, y) => x != y)
                .EqualityIsExpectedToBe(expectedIsEqual);
        }
    }
}