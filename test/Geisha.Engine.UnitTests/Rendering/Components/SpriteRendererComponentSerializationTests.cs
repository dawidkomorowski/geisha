using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
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
        protected override IComponentFactory ComponentFactory => new SpriteRendererComponentFactory();

        [Test]
        public void SerializeAndDeserialize_WhenSpriteIsNotNull()
        {
            // Arrange
            var texture = Substitute.For<ITexture>();
            var sprite = new Sprite(texture);
            var spriteAssetId = AssetId.CreateUnique();

            var component = new SpriteRendererComponent
            {
                Visible = false,
                SortingLayerName = "Some sorting layer",
                OrderInLayer = 2,
                Sprite = sprite
            };

            AssetStore.GetAssetId(sprite).Returns(spriteAssetId);
            AssetStore.GetAsset<Sprite>(spriteAssetId).Returns(sprite);

            // Act
            var actual = SerializeAndDeserialize(component);

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(component.Visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(component.SortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(component.OrderInLayer));
            Assert.That(actual.Sprite, Is.EqualTo(sprite));
        }

        [Test]
        public void SerializeAndDeserialize_WhenSpriteIsNull()
        {
            // Arrange
            var component = new SpriteRendererComponent
            {
                Visible = false,
                SortingLayerName = "Some sorting layer",
                OrderInLayer = 2,
                Sprite = null
            };

            // Act
            var actual = SerializeAndDeserialize(component);

            // Assert
            Assert.That(actual.Visible, Is.EqualTo(component.Visible));
            Assert.That(actual.SortingLayerName, Is.EqualTo(component.SortingLayerName));
            Assert.That(actual.OrderInLayer, Is.EqualTo(component.OrderInLayer));
            Assert.That(actual.Sprite, Is.Null);
        }
    }
}