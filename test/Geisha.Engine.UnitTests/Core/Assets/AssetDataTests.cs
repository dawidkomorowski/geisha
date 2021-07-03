using System.IO;
using Geisha.Engine.Core.Assets;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Assets
{
    public class AssetDataTests
    {
        [Test]
        public void Create_AssetData_WithBinaryContent()
        {
            // Arrange
            var assetId = AssetId.CreateUnique();
            var assetType = new AssetType("AssetType");
            var buffer = new byte[1000];
            Utils.Random.NextBytes(buffer);
            using var binaryContent = new MemoryStream(buffer);

            // Act
            var assetData = AssetData.Create(assetId, assetType, binaryContent);

            // Assert
            Assert.That(assetData.AssetId, Is.EqualTo(assetId));
            Assert.That(assetData.AssetType, Is.EqualTo(assetType));
            Assert.That(ReadToEnd(assetData.ReadBinaryContent()), Is.EqualTo(binaryContent));
        }

        [Test]
        public void Create_AssetData_WithStringContent()
        {
            // Arrange
            var assetId = AssetId.CreateUnique();
            var assetType = new AssetType("AssetType");
            const string stringContent = "StringContent";

            // Act
            var assetData = AssetData.Create(assetId, assetType, stringContent);

            // Assert
            Assert.That(assetData.AssetId, Is.EqualTo(assetId));
            Assert.That(assetData.AssetType, Is.EqualTo(assetType));
            Assert.That(assetData.ReadStringContent(), Is.EqualTo(stringContent));
        }

        [Test]
        public void Create_AssetData_WithJsonContent()
        {
            // Arrange
            var assetId = AssetId.CreateUnique();
            var assetType = new AssetType("AssetType");
            var jsonContent = new JsonContent
            {
                IntProperty = 123,
                DoubleProperty = 123.456,
                StringProperty = "String 123"
            };

            // Act
            var assetData = AssetData.Create(assetId, assetType, jsonContent);

            // Assert
            Assert.That(assetData.AssetId, Is.EqualTo(assetId));
            Assert.That(assetData.AssetType, Is.EqualTo(assetType));
            var actualJsonContent = assetData.ReadJsonContent<JsonContent>();
            Assert.That(actualJsonContent.IntProperty, Is.EqualTo(jsonContent.IntProperty));
            Assert.That(actualJsonContent.DoubleProperty, Is.EqualTo(jsonContent.DoubleProperty));
            Assert.That(actualJsonContent.StringProperty, Is.EqualTo(jsonContent.StringProperty));
        }

        [Test]
        public void SaveToStreamAndLoadFromStream_AssetData_WithBinaryContent()
        {
            var assetId = AssetId.CreateUnique();
            var assetType = new AssetType("AssetType");
            var buffer = new byte[1000];
            Utils.Random.NextBytes(buffer);
            using var binaryContent = new MemoryStream(buffer);

            var assetData = AssetData.Create(assetId, assetType, binaryContent);

            // Act
            using var memoryStream = new MemoryStream();
            assetData.Save(memoryStream);
            memoryStream.Position = 0;
            var actual = AssetData.Load(memoryStream);

            // Assert
            Assert.That(actual.AssetId, Is.EqualTo(assetId));
            Assert.That(actual.AssetType, Is.EqualTo(assetType));
            Assert.That(ReadToEnd(actual.ReadBinaryContent()), Is.EqualTo(binaryContent));
        }

        [Test]
        public void SaveToStreamAndLoadFromStream_AssetData_WithStringContent()
        {
            // Arrange
            var assetId = AssetId.CreateUnique();
            var assetType = new AssetType("AssetType");
            const string stringContent = "StringContent";

            var assetData = AssetData.Create(assetId, assetType, stringContent);

            // Act
            using var memoryStream = new MemoryStream();
            assetData.Save(memoryStream);
            memoryStream.Position = 0;
            var actual = AssetData.Load(memoryStream);

            // Assert
            Assert.That(actual.AssetId, Is.EqualTo(assetId));
            Assert.That(actual.AssetType, Is.EqualTo(assetType));
            Assert.That(actual.ReadStringContent(), Is.EqualTo(stringContent));
        }

        [Test]
        public void SaveToStreamAndLoadFromStream_AssetData_WithJsonContent()
        {
            // Arrange
            var assetId = AssetId.CreateUnique();
            var assetType = new AssetType("AssetType");
            var jsonContent = new JsonContent
            {
                IntProperty = 123,
                DoubleProperty = 123.456,
                StringProperty = "String 123"
            };

            var assetData = AssetData.Create(assetId, assetType, jsonContent);

            // Act
            using var memoryStream = new MemoryStream();
            assetData.Save(memoryStream);
            memoryStream.Position = 0;
            var actual = AssetData.Load(memoryStream);

            // Assert
            Assert.That(actual.AssetId, Is.EqualTo(assetId));
            Assert.That(actual.AssetType, Is.EqualTo(assetType));
            var actualJsonContent = actual.ReadJsonContent<JsonContent>();
            Assert.That(actualJsonContent.IntProperty, Is.EqualTo(jsonContent.IntProperty));
            Assert.That(actualJsonContent.DoubleProperty, Is.EqualTo(jsonContent.DoubleProperty));
            Assert.That(actualJsonContent.StringProperty, Is.EqualTo(jsonContent.StringProperty));
        }

        private static byte[] ReadToEnd(Stream stream)
        {
            using var streamReader = new BinaryReader(stream);
            return streamReader.ReadBytes((int) stream.Length);
        }

        private sealed class JsonContent
        {
            public int IntProperty { get; set; }
            public double DoubleProperty { get; set; }
            public string? StringProperty { get; set; }
        }
    }
}