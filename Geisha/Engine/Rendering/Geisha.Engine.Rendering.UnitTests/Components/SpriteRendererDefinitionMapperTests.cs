using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.Rendering.Components.Definition;
using Geisha.Framework.Rendering;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Rendering.UnitTests.Components
{
    [TestFixture]
    public class SpriteRendererDefinitionMapperTests
    {
        private IAssetStore _assetStore;
        private SpriteRendererDefinitionMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _assetStore = Substitute.For<IAssetStore>();
            _mapper = new SpriteRendererDefinitionMapper(_assetStore);
        }

        [Test]
        public void ToDefinition()
        {
            // Arrange
            var sprite = new Sprite();
            var spriteAssetId = Guid.NewGuid();

            var spriteRenderer = new SpriteRenderer
            {
                Visible = true,
                SortingLayerName = "Some sorting layer",
                OrderInLayer = 2,
                Sprite = sprite
            };

            _assetStore.GetAssetId(sprite).Returns(spriteAssetId);

            // Act
            var actual = (SpriteRendererDefinition) _mapper.ToDefinition(spriteRenderer);

            // Assert
            Assert.That(actual.Visible, Is.True);
            Assert.That(actual.SortingLayerName, Is.EqualTo("Some sorting layer"));
            Assert.That(actual.OrderInLayer, Is.EqualTo(2));
            Assert.That(actual.SpriteAssetId, Is.EqualTo(spriteAssetId));
        }

        [Test]
        public void FromDefinition()
        {
            // Arrange
            var sprite = new Sprite();
            var spriteAssetId = Guid.NewGuid();

            var spriteRendererDefinition = new SpriteRendererDefinition
            {
                Visible = true,
                SortingLayerName = "Some sorting layer",
                OrderInLayer = 2,
                SpriteAssetId = spriteAssetId
            };

            _assetStore.GetAsset<Sprite>(spriteAssetId).Returns(sprite);

            // Act
            var actual = (SpriteRenderer) _mapper.FromDefinition(spriteRendererDefinition);

            // Assert
            Assert.That(actual.Visible, Is.True);
            Assert.That(actual.SortingLayerName, Is.EqualTo("Some sorting layer"));
            Assert.That(actual.OrderInLayer, Is.EqualTo(2));
            Assert.That(actual.Sprite, Is.EqualTo(sprite));
        }
    }
}