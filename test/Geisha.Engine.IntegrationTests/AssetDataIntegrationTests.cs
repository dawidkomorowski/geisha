using System.IO;
using Geisha.Engine.Core.Assets;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.IntegrationTests
{
    [TestFixture]
    public class AssetDataIntegrationTests
    {
        private TemporaryDirectory _temporaryDirectory = null!;
        private AssetId _assetId;
        private AssetType _assetType;
        private byte[] _buffer = null!;
        private Stream _binaryContent = null!;
        private const string StringContent = "StringContent";
        private JsonContent _jsonContent = null!;

        [SetUp]
        public void SetUp()
        {
            _temporaryDirectory = new TemporaryDirectory();

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
            _temporaryDirectory.Dispose();
        }

        [Test]
        public void SaveToFileAndLoadFromFile_AssetData_WithBinaryContent()
        {
            // Arrange
            var assetData = AssetData.CreateWithBinaryContent(_assetId, _assetType, _binaryContent);

            // Act
            var filePath = _temporaryDirectory.GetRandomFilePath();
            assetData.Save(filePath);
            var actual = AssetData.Load(filePath);

            // Assert
            Assert.That(actual.AssetId, Is.EqualTo(_assetId));
            Assert.That(actual.AssetType, Is.EqualTo(_assetType));
            Assert.That(assetData.ContentFormat, Is.EqualTo(AssetData.AssetContentFormat.Binary));
            Assert.That(ReadToEnd(actual.ReadBinaryContent()), Is.EqualTo(_buffer));
        }

        [Test]
        public void SaveToFileAndLoadFromFile_AssetData_WithStringContent()
        {
            // Arrange
            var assetData = AssetData.CreateWithStringContent(_assetId, _assetType, StringContent);

            // Act
            var filePath = _temporaryDirectory.GetRandomFilePath();
            assetData.Save(filePath);
            var actual = AssetData.Load(filePath);

            // Assert
            Assert.That(actual.AssetId, Is.EqualTo(_assetId));
            Assert.That(actual.AssetType, Is.EqualTo(_assetType));
            Assert.That(assetData.ContentFormat, Is.EqualTo(AssetData.AssetContentFormat.String));
            Assert.That(actual.ReadStringContent(), Is.EqualTo(StringContent));
        }

        [Test]
        public void SaveToFileAndLoadFromFile_AssetData_WithJsonContent()
        {
            // Arrange
            var assetData = AssetData.CreateWithJsonContent(_assetId, _assetType, _jsonContent);

            // Act
            var filePath = _temporaryDirectory.GetRandomFilePath();
            assetData.Save(filePath);
            var actual = AssetData.Load(filePath);

            // Assert
            Assert.That(actual.AssetId, Is.EqualTo(_assetId));
            Assert.That(actual.AssetType, Is.EqualTo(_assetType));
            Assert.That(assetData.ContentFormat, Is.EqualTo(AssetData.AssetContentFormat.Json));
            var actualJsonContent = actual.ReadJsonContent<JsonContent>();
            Assert.That(actualJsonContent.IntProperty, Is.EqualTo(_jsonContent.IntProperty));
            Assert.That(actualJsonContent.DoubleProperty, Is.EqualTo(_jsonContent.DoubleProperty));
            Assert.That(actualJsonContent.StringProperty, Is.EqualTo(_jsonContent.StringProperty));
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