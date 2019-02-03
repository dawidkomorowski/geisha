using System;
using Geisha.Engine.Core.Assets;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Assets
{
    [TestFixture]
    public class AssetIdTests
    {
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

        [TestCase("7DE5D927-3AC9-45DB-9DFF-2F5DC8595E3E", "7DE5D927-3AC9-45DB-9DFF-2F5DC8595E3E", true)]
        [TestCase("7DE5D927-3AC9-45DB-9DFF-2F5DC8595E3E", "7956026B-8D34-453A-A2F4-8D3E4D5D04E6", false)]
        public void EqualityMembers_AreImplementedCorrectly(string guid1, string guid2, bool expectedEqual)
        {
            // Arrange
            var assetId1 = new AssetId(Guid.Parse(guid1));
            var assetId2 = new AssetId(Guid.Parse(guid2));

            // Act
            var equalsTyped = assetId1.Equals(assetId2);
            var equals = assetId1.Equals((object) assetId2);
            var equalsNull = assetId1.Equals(null);
            var getHashCode = assetId1.GetHashCode() == assetId2.GetHashCode();
            var equalityOperator = assetId1 == assetId2;
            var inequalityOperator = assetId1 != assetId2;

            // Assert
            Assert.That(equalsTyped, Is.EqualTo(expectedEqual));
            Assert.That(equals, Is.EqualTo(expectedEqual));
            Assert.That(equalsNull, Is.False);
            Assert.That(getHashCode, Is.EqualTo(expectedEqual));
            Assert.That(equalityOperator, Is.EqualTo(expectedEqual));
            Assert.That(inequalityOperator, Is.EqualTo(!expectedEqual));
        }

        [Test]
        [SetCulture("")]
        public void ToString_Test()
        {
            // Arrange
            var assetId = new AssetId(Guid.Parse("555c5920-9d6a-463b-80c0-7aaffc6e4caf"));

            // Act
            var actual = assetId.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo("Value: 555c5920-9d6a-463b-80c0-7aaffc6e4caf"));
        }
    }
}