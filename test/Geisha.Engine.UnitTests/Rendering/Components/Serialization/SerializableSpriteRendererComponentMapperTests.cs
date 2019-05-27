using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Components.Serialization;
using Geisha.Framework.Rendering;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components.Serialization
{
    [TestFixture]
    public class SerializableSpriteRendererComponentMapperTests
    {
        private IAssetStore _assetStore;
        private SerializableSpriteRendererComponentMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _assetStore = Substitute.For<IAssetStore>();
            _mapper = new SerializableSpriteRendererComponentMapper(_assetStore);
        }

        [Test]
        public void MapToSerializable()
        {
            // Arrange
            var sprite = new Sprite();
            var spriteAssetId = AssetId.CreateUnique();

            var spriteRenderer = new SpriteRendererComponent
            {
                Visible = true,
                SortingLayerName = "Some sorting layer",
                OrderInLayer = 2,
                Sprite = sprite
            };

            _assetStore.GetAssetId(sprite).Returns(spriteAssetId);

            // Act
            var actual = (SerializableSpriteRendererComponent) _mapper.MapToSerializable(spriteRenderer);

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(spriteRenderer.Visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(spriteRenderer.SortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(spriteRenderer.OrderInLayer));
            Assert.That(actual.SpriteAssetId, Is.EqualTo(spriteAssetId.Value));
        }

        [Test]
        public void MapFromSerializable()
        {
            // Arrange
            var sprite = new Sprite();
            var spriteAssetId = AssetId.CreateUnique();

            var serializableSpriteRenderer = new SerializableSpriteRendererComponent
            {
                Visible = true,
                SortingLayerName = "Some sorting layer",
                OrderInLayer = 2,
                SpriteAssetId = spriteAssetId.Value
            };

            _assetStore.GetAsset<Sprite>(spriteAssetId).Returns(sprite);

            // Act
            var actual = (SpriteRendererComponent) _mapper.MapFromSerializable(serializableSpriteRenderer);

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(serializableSpriteRenderer.Visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(serializableSpriteRenderer.SortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(serializableSpriteRenderer.OrderInLayer));
            Assert.That(actual.Sprite, Is.EqualTo(sprite));
        }
    }
}