using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components
{
    [TestFixture]
    public class SpriteRendererComponentSerializationTests : ComponentSerializationTestsBase
    {
        [Test]
        public void SerializeAndDeserialize_WhenSpriteIsNotNull()
        {
            // Arrange
            const bool visible = false;
            const string sortingLayerName = "Some sorting layer";
            const int orderInLayer = 2;
            const double opacity = 0.5;
            const BitmapInterpolationMode bitmapInterpolationMode = BitmapInterpolationMode.NearestNeighbor;

            var texture = Substitute.For<ITexture>();
            var sprite = new Sprite(texture, Vector2.Zero, Vector2.Zero, Vector2.Zero, 0);
            var spriteAssetId = AssetId.CreateUnique();

            AssetStore.GetAssetId(sprite).Returns(spriteAssetId);
            AssetStore.GetAsset<Sprite>(spriteAssetId).Returns(sprite);

            // Act
            var actual = SerializeAndDeserialize<SpriteRendererComponent>(component =>
            {
                component.Visible = visible;
                component.SortingLayerName = sortingLayerName;
                component.OrderInLayer = orderInLayer;
                component.Sprite = sprite;
                component.Opacity = opacity;
                component.BitmapInterpolationMode = bitmapInterpolationMode;
            });

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(sortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(orderInLayer));
            Assert.That(actual.Sprite, Is.EqualTo(sprite));
            Assert.That(actual.Opacity, Is.EqualTo(opacity));
            Assert.That(actual.BitmapInterpolationMode, Is.EqualTo(bitmapInterpolationMode));
        }

        [Test]
        public void SerializeAndDeserialize_WhenSpriteIsNull()
        {
            // Arrange
            const bool visible = false;
            const string sortingLayerName = "Some sorting layer";
            const int orderInLayer = 2;
            const double opacity = 0.5;
            const BitmapInterpolationMode bitmapInterpolationMode = BitmapInterpolationMode.NearestNeighbor;

            // Act
            var actual = SerializeAndDeserialize<SpriteRendererComponent>(component =>
            {
                component.Visible = visible;
                component.SortingLayerName = sortingLayerName;
                component.OrderInLayer = orderInLayer;
                component.Sprite = null;
                component.Opacity = opacity;
                component.BitmapInterpolationMode = bitmapInterpolationMode;
            });

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(sortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(orderInLayer));
            Assert.That(actual.Sprite, Is.Null);
            Assert.That(actual.Opacity, Is.EqualTo(opacity));
            Assert.That(actual.BitmapInterpolationMode, Is.EqualTo(bitmapInterpolationMode));
        }
    }
}