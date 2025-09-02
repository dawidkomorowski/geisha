using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Systems.RenderingSystemTests;

[TestFixture]
public class RenderingOrderTests : RenderingSystemTestsBase
{
    [Test]
    public void RenderScene_ShouldIgnoreOrderInLayer_WhenEntitiesAreInDifferentSortingLayers()
    {
        // Arrange
        const string otherSortingLayer = "Other";

        var context = CreateRenderingTestContext(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, otherSortingLayer } });
        context.AddCamera();
        var entity1 = context.AddSprite(orderInLayer: 0, sortingLayerName: otherSortingLayer);
        var entity2 = context.AddSprite(orderInLayer: 1);

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        Received.InOrder(() =>
        {
            RenderingContext2D.DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
            RenderingContext2D.DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
        });
    }

    [Test]
    public void RenderScene_ShouldRenderInOrderOf_OrderInLayer_WhenEntitiesAreInTheSameSortingLayer()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var entity1 = renderingScene.AddSprite(orderInLayer: 1);
        var entity2 = renderingScene.AddSprite(orderInLayer: -1);
        var entity3 = renderingScene.AddSprite(orderInLayer: 0);

        // Act
        renderingSystem.RenderScene();

        // Assert
        Received.InOrder(() =>
        {
            RenderingContext2D.DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
            RenderingContext2D.DrawSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix(), entity3.GetOpacity());
            RenderingContext2D.DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
        });
    }

    [Test]
    public void RenderScene_ShouldRenderInSortingLayersOrder_Default_Background_Foreground()
    {
        // Arrange
        const string backgroundSortingLayerName = "Background";
        const string foregroundSortingLayerName = "Foreground";

        var context = CreateRenderingTestContext(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, backgroundSortingLayerName, foregroundSortingLayerName } });
        context.AddCamera();
        var entity1 = context.AddSprite(sortingLayerName: foregroundSortingLayerName);
        var entity2 = context.AddSprite(sortingLayerName: RenderingConfiguration.DefaultSortingLayerName);
        var entity3 = context.AddSprite(sortingLayerName: backgroundSortingLayerName);

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        Received.InOrder(() =>
        {
            RenderingContext2D.DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
            RenderingContext2D.DrawSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix(), entity3.GetOpacity());
            RenderingContext2D.DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
        });
    }

    [Test]
    public void RenderScene_ShouldRenderInSortingLayersOrder_Foreground_Background_Default()
    {
        // Arrange
        const string backgroundSortingLayerName = "Background";
        const string foregroundSortingLayerName = "Foreground";

        var context = CreateRenderingTestContext(new RenderingConfiguration
            { SortingLayersOrder = new[] { foregroundSortingLayerName, backgroundSortingLayerName, RenderingConfiguration.DefaultSortingLayerName } });
        context.AddCamera();
        var entity1 = context.AddSprite(sortingLayerName: foregroundSortingLayerName);
        var entity2 = context.AddSprite(sortingLayerName: RenderingConfiguration.DefaultSortingLayerName);
        var entity3 = context.AddSprite(sortingLayerName: backgroundSortingLayerName);

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        Received.InOrder(() =>
        {
            RenderingContext2D.DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
            RenderingContext2D.DrawSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix(), entity3.GetOpacity());
            RenderingContext2D.DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
        });
    }

    [Test]
    public void RenderScene_ShouldDrawSpriteBatchWithSpritesSortedBySortingLayers()
    {
        // Arrange
        const string backgroundSortingLayerName = "Background";
        const string foregroundSortingLayerName = "Foreground";

        var context = CreateRenderingTestContext(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, backgroundSortingLayerName, foregroundSortingLayerName } });
        context.AddCamera();

        var entity1 = context.AddSprite(sortingLayerName: foregroundSortingLayerName);
        var entity2 = context.AddSprite(sortingLayerName: RenderingConfiguration.DefaultSortingLayerName);
        var entity3 = context.AddSprite(sortingLayerName: backgroundSortingLayerName);

        var texture = CreateTexture();
        entity1.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);
        entity2.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);
        entity3.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);


        RenderingContext2D.When(x => x.DrawSpriteBatch(Arg.Any<SpriteBatch>())).Do(x =>
        {
            // Assert
            var spriteBatch = x.Arg<SpriteBatch>();
            Assert.That(spriteBatch.Count, Is.EqualTo(3));
            Assert.That(spriteBatch.Texture, Is.EqualTo(texture));

            var sprites = spriteBatch.GetSpritesSpan().ToArray();
            Assert.That(sprites[0].Sprite, Is.EqualTo(entity2.GetSprite()));
            Assert.That(sprites[1].Sprite, Is.EqualTo(entity3.GetSprite()));
            Assert.That(sprites[2].Sprite, Is.EqualTo(entity1.GetSprite()));
        });

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSpriteBatch(Arg.Any<SpriteBatch>());
    }

    [Test]
    public void RenderScene_ShouldDrawSpriteBatchWithSpritesSortedByOrderInLayer()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();

        var entity1 = renderingScene.AddSprite(orderInLayer: 1);
        var entity2 = renderingScene.AddSprite(orderInLayer: -1);
        var entity3 = renderingScene.AddSprite(orderInLayer: 0);

        var texture = CreateTexture();
        entity1.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);
        entity2.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);
        entity3.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);


        RenderingContext2D.When(x => x.DrawSpriteBatch(Arg.Any<SpriteBatch>())).Do(x =>
        {
            // Assert
            var spriteBatch = x.Arg<SpriteBatch>();
            Assert.That(spriteBatch.Count, Is.EqualTo(3));
            Assert.That(spriteBatch.Texture, Is.EqualTo(texture));

            var sprites = spriteBatch.GetSpritesSpan().ToArray();
            Assert.That(sprites[0].Sprite, Is.EqualTo(entity2.GetSprite()));
            Assert.That(sprites[1].Sprite, Is.EqualTo(entity3.GetSprite()));
            Assert.That(sprites[2].Sprite, Is.EqualTo(entity1.GetSprite()));
        });

        // Act
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSpriteBatch(Arg.Any<SpriteBatch>());
    }

    [Test]
    public void RenderScene_ShouldDrawSpriteBatchWithSpritesSortedBySortingLayers_DespiteOrderInLayer()
    {
        // Arrange
        const string otherSortingLayer = "Other";

        var context = CreateRenderingTestContext(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, otherSortingLayer } });
        context.AddCamera();

        var entity1 = context.AddSprite(orderInLayer: 0, sortingLayerName: otherSortingLayer);
        var entity2 = context.AddSprite(orderInLayer: 1);

        var texture = CreateTexture();
        entity1.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);
        entity2.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);


        RenderingContext2D.When(x => x.DrawSpriteBatch(Arg.Any<SpriteBatch>())).Do(x =>
        {
            // Assert
            var spriteBatch = x.Arg<SpriteBatch>();
            Assert.That(spriteBatch.Count, Is.EqualTo(2));
            Assert.That(spriteBatch.Texture, Is.EqualTo(texture));

            var sprites = spriteBatch.GetSpritesSpan().ToArray();
            Assert.That(sprites[0].Sprite, Is.EqualTo(entity2.GetSprite()));
            Assert.That(sprites[1].Sprite, Is.EqualTo(entity1.GetSprite()));
        });

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSpriteBatch(Arg.Any<SpriteBatch>());
    }
}