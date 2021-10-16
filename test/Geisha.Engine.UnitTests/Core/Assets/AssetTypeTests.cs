using Geisha.Engine.Core.Assets;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Assets
{
    public class AssetTypeTests
    {
        [Test]
        public void Constructor_CreatesAssetTypeWithEmptyString_GivenNoParameters()
        {
            // Arrange
            // Act
            var assetType = new AssetType();
            var actual = assetType.Value;

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void Constructor_ThrowsException_GivenNull()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => new AssetType(null!), Throws.ArgumentNullException);
        }

        [Test]
        public void Constructor_CreatesAssetTypeWithValue_GivenNonNullString()
        {
            // Arrange
            const string assetTypeString = "Some Asset Type";

            // Act
            var assetType = new AssetType(assetTypeString);
            var actual = assetType.Value;

            // Assert
            Assert.That(actual, Is.EqualTo(assetTypeString));
        }

        [Test]
        public void ToString_ShouldReturnValue()
        {
            // Arrange
            const string assetTypeString = "Some Asset Type";
            var assetType = new AssetType(assetTypeString);

            // Act
            var actual = assetType.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo(assetTypeString));
        }

        [TestCase("AssetType 1", "AssetType 1", true)]
        [TestCase("AssetType 1", "AssetType 2", false)]
        public void EqualityMembers_ShouldEqualAssetType_WhenStringValueIsEqual(string type1, string type2, bool expectedIsEqual)
        {
            // Arrange
            var assetType1 = new AssetType(type1);
            var assetType2 = new AssetType(type2);

            // Act
            // Assert
            AssertEqualityMembers
                .ForValues(assetType1, assetType2)
                .UsingEqualityOperator((x, y) => x == y)
                .UsingInequalityOperator((x, y) => x != y)
                .EqualityIsExpectedToBe(expectedIsEqual);
        }
    }
}