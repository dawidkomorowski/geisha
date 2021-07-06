using System.IO;
using Geisha.Engine.Core.Assets;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Assets
{
    public class AssetDataTests
    {
        private AssetId _assetId;
        private AssetType _assetType;
        private byte[] _buffer = null!;
        private Stream _binaryContent = null!;
        private const string StringContent = "StringContent";
        private JsonContent _jsonContent = null!;

        [SetUp]
        public void SetUp()
        {
            _assetId = AssetId.CreateUnique();
            _assetType = new AssetType("AssetType");
            _buffer = new byte[1000];
            Utils.Random.NextBytes(_buffer);
            _binaryContent = new MemoryStream(_buffer);
            _jsonContent = new JsonContent
            {
                IntProperty = 123,
                DoubleProperty = 123.456,
                StringProperty = "String 123"
            };
        }

        [TearDown]
        public void TearDown()
        {
            _binaryContent.Dispose();
        }

        [Test]
        public void Create_AssetData_WithBinaryContent()
        {
            // Arrange
            // Act
            var assetData = AssetData.CreateWithBinaryContent(_assetId, _assetType, _binaryContent);

            // Assert
            Assert.That(assetData.AssetId, Is.EqualTo(_assetId));
            Assert.That(assetData.AssetType, Is.EqualTo(_assetType));
            Assert.That(assetData.ContentType, Is.EqualTo(AssetData.AssetContentType.Binary));
            Assert.That(ReadToEnd(assetData.ReadBinaryContent()), Is.EqualTo(_buffer));
        }

        [Test]
        public void Create_AssetData_WithStringContent()
        {
            // Arrange
            // Act
            var assetData = AssetData.CreateWithStringContent(_assetId, _assetType, StringContent);

            // Assert
            Assert.That(assetData.AssetId, Is.EqualTo(_assetId));
            Assert.That(assetData.AssetType, Is.EqualTo(_assetType));
            Assert.That(assetData.ContentType, Is.EqualTo(AssetData.AssetContentType.String));
            Assert.That(assetData.ReadStringContent(), Is.EqualTo(StringContent));
        }

        [Test]
        public void Create_AssetData_WithJsonContent()
        {
            // Arrange
            // Act
            var assetData = AssetData.CreateWithJsonContent(_assetId, _assetType, _jsonContent);

            // Assert
            Assert.That(assetData.AssetId, Is.EqualTo(_assetId));
            Assert.That(assetData.AssetType, Is.EqualTo(_assetType));
            Assert.That(assetData.ContentType, Is.EqualTo(AssetData.AssetContentType.Json));
            var actualJsonContent = assetData.ReadJsonContent<JsonContent>();
            Assert.That(actualJsonContent.IntProperty, Is.EqualTo(_jsonContent.IntProperty));
            Assert.That(actualJsonContent.DoubleProperty, Is.EqualTo(_jsonContent.DoubleProperty));
            Assert.That(actualJsonContent.StringProperty, Is.EqualTo(_jsonContent.StringProperty));
        }

        [Test]
        public void SaveToStreamAndLoadFromStream_AssetData_WithBinaryContent()
        {
            // Arrange
            var assetData = AssetData.CreateWithBinaryContent(_assetId, _assetType, _binaryContent);

            // Act
            using var memoryStream = new MemoryStream();
            assetData.Save(memoryStream);
            memoryStream.Position = 0;
            var actual = AssetData.Load(memoryStream);

            // Assert
            Assert.That(actual.AssetId, Is.EqualTo(_assetId));
            Assert.That(actual.AssetType, Is.EqualTo(_assetType));
            Assert.That(assetData.ContentType, Is.EqualTo(AssetData.AssetContentType.Binary));
            Assert.That(ReadToEnd(actual.ReadBinaryContent()), Is.EqualTo(_buffer));
        }

        [Test]
        public void SaveToStreamAndLoadFromStream_AssetData_WithStringContent()
        {
            // Arrange
            var assetData = AssetData.CreateWithStringContent(_assetId, _assetType, StringContent);

            // Act
            using var memoryStream = new MemoryStream();
            assetData.Save(memoryStream);
            memoryStream.Position = 0;
            var actual = AssetData.Load(memoryStream);

            // Assert
            Assert.That(actual.AssetId, Is.EqualTo(_assetId));
            Assert.That(actual.AssetType, Is.EqualTo(_assetType));
            Assert.That(assetData.ContentType, Is.EqualTo(AssetData.AssetContentType.String));
            Assert.That(actual.ReadStringContent(), Is.EqualTo(StringContent));
        }

        [Test]
        public void SaveToStreamAndLoadFromStream_AssetData_WithJsonContent()
        {
            // Arrange
            var assetData = AssetData.CreateWithJsonContent(_assetId, _assetType, _jsonContent);

            // Act
            using var memoryStream = new MemoryStream();
            assetData.Save(memoryStream);
            memoryStream.Position = 0;
            var actual = AssetData.Load(memoryStream);

            // Assert
            Assert.That(actual.AssetId, Is.EqualTo(_assetId));
            Assert.That(actual.AssetType, Is.EqualTo(_assetType));
            Assert.That(assetData.ContentType, Is.EqualTo(AssetData.AssetContentType.Json));
            var actualJsonContent = actual.ReadJsonContent<JsonContent>();
            Assert.That(actualJsonContent.IntProperty, Is.EqualTo(_jsonContent.IntProperty));
            Assert.That(actualJsonContent.DoubleProperty, Is.EqualTo(_jsonContent.DoubleProperty));
            Assert.That(actualJsonContent.StringProperty, Is.EqualTo(_jsonContent.StringProperty));
        }

        [Test]
        public void ReadBinaryContent_ShouldThrowException_WhenContentIsString()
        {
            // Arrange
            // Act
            var assetData = AssetData.CreateWithStringContent(_assetId, _assetType, StringContent);

            // Assert
            Assert.That(() => assetData.ReadBinaryContent(), Throws.InvalidOperationException);
        }

        [Test]
        public void ReadBinaryContent_ShouldThrowException_WhenContentIsJson()
        {
            // Arrange
            // Act
            var assetData = AssetData.CreateWithJsonContent(_assetId, _assetType, _jsonContent);

            // Assert
            Assert.That(() => assetData.ReadBinaryContent(), Throws.InvalidOperationException);
        }

        [Test]
        public void ReadStringContent_ShouldThrowException_WhenContentIsBinary()
        {
            // Arrange
            // Act
            var assetData = AssetData.CreateWithBinaryContent(_assetId, _assetType, _binaryContent);

            // Assert
            Assert.That(() => assetData.ReadStringContent(), Throws.InvalidOperationException);
        }

        [Test]
        public void ReadStringContent_ShouldThrowException_WhenContentIsJson()
        {
            // Arrange
            // Act
            var assetData = AssetData.CreateWithJsonContent(_assetId, _assetType, _jsonContent);

            // Assert
            Assert.That(() => assetData.ReadStringContent(), Throws.InvalidOperationException);
        }

        [Test]
        public void ReadJsonContent_ShouldThrowException_WhenContentIsBinary()
        {
            // Arrange
            // Act
            var assetData = AssetData.CreateWithBinaryContent(_assetId, _assetType, _binaryContent);

            // Assert
            Assert.That(() => assetData.ReadJsonContent<JsonContent>(), Throws.InvalidOperationException);
        }

        [Test]
        public void ReadJsonContent_ShouldThrowException_WhenContentIsString()
        {
            // Arrange
            // Act
            var assetData = AssetData.CreateWithStringContent(_assetId, _assetType, StringContent);

            // Assert
            Assert.That(() => assetData.ReadJsonContent<JsonContent>(), Throws.InvalidOperationException);
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