using System;
using Geisha.Engine.Core.Assets;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Assets
{
    [TestFixture]
    public class AssetIdTests
    {
        [Test]
        public void CreateUnique_CreatesUniqueAssetIds()
        {
            // Arrange
            // Act
            var assetId1 = AssetId.CreateUnique();
            var assetId2 = AssetId.CreateUnique();

            // Assert
            Assert.That(assetId1, Is.Not.EqualTo(assetId2));
        }

        [Test]
        public void Constructor_CreatesAssetIdWithValueGivenAsParameter()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var actual = new AssetId(guid);

            // Assert
            Assert.That(actual.Value, Is.EqualTo(guid));
        }

        [Test]
        public void ToString_ShouldReturn_Guid()
        {
            // Arrange
            var assetId = new AssetId(new Guid("7BE324F2-25A0-4C85-9B85-069B16B0B84F"));

            // Act
            var actual = assetId.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo("7BE324F2-25A0-4C85-9B85-069B16B0B84F"));
        }

        [TestCase("7DE5D927-3AC9-45DB-9DFF-2F5DC8595E3E", "7DE5D927-3AC9-45DB-9DFF-2F5DC8595E3E", true)]
        [TestCase("7DE5D927-3AC9-45DB-9DFF-2F5DC8595E3E", "7956026B-8D34-453A-A2F4-8D3E4D5D04E6", false)]
        public void EqualityMembers_ShouldEqualAssetId_WhenGuidIsEqual(string guid1, string guid2, bool expectedIsEqual)
        {
            // Arrange
            var assetId1 = new AssetId(new Guid(guid1));
            var assetId2 = new AssetId(new Guid(guid2));

            // Act
            // Assert
            Assert.That(assetId1.Equals(assetId2), Is.EqualTo(expectedIsEqual));
            Assert.That(assetId2.Equals(assetId1), Is.EqualTo(expectedIsEqual));
            Assert.That(assetId1.Equals((object) assetId2), Is.EqualTo(expectedIsEqual));
            Assert.That(assetId2.Equals((object) assetId1), Is.EqualTo(expectedIsEqual));
            Assert.That(assetId1 == assetId2, Is.EqualTo(expectedIsEqual));
            Assert.That(assetId2 == assetId1, Is.EqualTo(expectedIsEqual));
            Assert.That(assetId1 != assetId2, Is.EqualTo(!expectedIsEqual));
            Assert.That(assetId2 != assetId1, Is.EqualTo(!expectedIsEqual));
            Assert.That(assetId1.GetHashCode().Equals(assetId2.GetHashCode()), Is.EqualTo(expectedIsEqual));

            Assert.That(assetId1.Equals(null), Is.False);
            Assert.That(assetId2.Equals(null), Is.False);
        }
    }
}