using System.Linq;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Systems.RenderingSystemTests;

[TestFixture]
public class SpriteRendererComponentTests : RenderingSystemTestsBase
{
    [Test]
    public void RenderingSystem_ShouldKeepSpriteRendererComponentState_WhenRenderingSystemIsRemovedFromSceneObserversOfScene()
    {
        // Arrange
        // Renderer2DComponent
        const bool visible = false;
        const string sortingLayerName = "some sorting layer";
        const int orderInLayer = 12;
        // SpriteRendererComponent
        var sprite = new Sprite(Substitute.For<ITexture>(), Vector2.Zero, Vector2.Zero, Vector2.Zero, 0);
        const double opacity = 0.5;

        var (renderingSystem, renderingScene) = GetRenderingSystem(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, sortingLayerName } });

        var entity = renderingScene.AddSprite();
        var spriteRendererComponent = entity.GetComponent<SpriteRendererComponent>();

        // Renderer2DComponent
        spriteRendererComponent.Visible = visible;
        spriteRendererComponent.SortingLayerName = sortingLayerName;
        spriteRendererComponent.OrderInLayer = orderInLayer;
        // SpriteRendererComponent
        spriteRendererComponent.Sprite = sprite;
        spriteRendererComponent.Opacity = opacity;

        // Assume
        Assert.That(spriteRendererComponent.IsManagedByRenderingSystem, Is.True);

        // Act
        renderingScene.Scene.RemoveObserver(renderingSystem);

        // Assert
        // Renderer2DComponent
        Assert.That(spriteRendererComponent.IsManagedByRenderingSystem, Is.False);
        Assert.That(spriteRendererComponent.Visible, Is.EqualTo(visible));
        Assert.That(spriteRendererComponent.SortingLayerName, Is.EqualTo(sortingLayerName));
        Assert.That(spriteRendererComponent.OrderInLayer, Is.EqualTo(orderInLayer));
        // SpriteRendererComponent
        Assert.That(spriteRendererComponent.Sprite, Is.EqualTo(sprite));
        Assert.That(spriteRendererComponent.Opacity, Is.EqualTo(opacity));
    }

    [Test]
    public void RenderingSystem_ShouldKeepSpriteRendererComponentState_WhenRenderingSystemIsAddedToSceneObserversOfScene()
    {
        // Arrange
        // Renderer2DComponent
        const bool visible = false;
        const string sortingLayerName = "some sorting layer";
        const int orderInLayer = 12;
        // SpriteRendererComponent
        var sprite = new Sprite(Substitute.For<ITexture>(), Vector2.Zero, Vector2.Zero, Vector2.Zero, 0);
        const double opacity = 0.5;

        var (renderingSystem, renderingScene) = GetRenderingSystem(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, sortingLayerName } });
        renderingScene.Scene.RemoveObserver(renderingSystem);

        var entity = renderingScene.AddSprite();
        var spriteRendererComponent = entity.GetComponent<SpriteRendererComponent>();

        // Renderer2DComponent
        spriteRendererComponent.Visible = visible;
        spriteRendererComponent.SortingLayerName = sortingLayerName;
        spriteRendererComponent.OrderInLayer = orderInLayer;
        // SpriteRendererComponent
        spriteRendererComponent.Sprite = sprite;
        spriteRendererComponent.Opacity = opacity;

        // Assume
        Assert.That(spriteRendererComponent.IsManagedByRenderingSystem, Is.False);

        // Act
        renderingScene.Scene.AddObserver(renderingSystem);

        // Assert
        // Renderer2DComponent
        Assert.That(spriteRendererComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(spriteRendererComponent.Visible, Is.EqualTo(visible));
        Assert.That(spriteRendererComponent.SortingLayerName, Is.EqualTo(sortingLayerName));
        Assert.That(spriteRendererComponent.OrderInLayer, Is.EqualTo(orderInLayer));
        // SpriteRendererComponent
        Assert.That(spriteRendererComponent.Sprite, Is.EqualTo(sprite));
        Assert.That(spriteRendererComponent.Opacity, Is.EqualTo(opacity));
    }

    [Test]
    public void RenderScene_ShouldDrawSprite_WhenSceneContainsEntityWithSpriteRendererAndTransform()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var entity = renderingScene.AddSprite();

        // Act
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(entity.GetSprite(), entity.Get2DTransformationMatrix(), entity.GetOpacity());
    }

    [Test]
    public void RenderScene_ShouldDrawSprite_WhenOpacityIsNonDefault()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();
        var entity = renderingScene.AddSprite();
        var spriteRendererComponent = entity.GetComponent<SpriteRendererComponent>();
        spriteRendererComponent.Opacity = 0.5;

        // Act
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(entity.GetSprite(), entity.Get2DTransformationMatrix(), 0.5);
    }

    [Test]
    public void RenderScene_ShouldDrawSpriteBatch_WhenSceneContainsTwoSpritesWithTheSameTexture()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();

        var entity1 = renderingScene.AddSprite(translation: new Vector2(10, 20), opacity: 1d);
        var entity2 = renderingScene.AddSprite(translation: new Vector2(30, 40), opacity: 0.5d);

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
            var spriteBatchElement1 = sprites.Single(s => s.Sprite == entity1.GetSprite());
            var spriteBatchElement2 = sprites.Single(s => s.Sprite == entity2.GetSprite());

            Assert.That(spriteBatchElement1.Transform, Is.EqualTo(entity1.Get2DTransformationMatrix()));
            Assert.That(spriteBatchElement1.Opacity, Is.EqualTo(entity1.GetOpacity()));

            Assert.That(spriteBatchElement2.Transform, Is.EqualTo(entity2.Get2DTransformationMatrix()));
            Assert.That(spriteBatchElement2.Opacity, Is.EqualTo(entity2.GetOpacity()));
        });

        // Act
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSpriteBatch(Arg.Any<SpriteBatch>());
    }

    [Test]
    public void RenderScene_ShouldDrawSpriteBatchAndDrawSprite_WhenSceneContainsTwoSpritesWithTheSameTextureAndOneSpriteWithDifferentTexture()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();

        var entity1 = renderingScene.AddSprite();
        var entity2 = renderingScene.AddSprite();
        var entity3 = renderingScene.AddSprite();

        var texture = CreateTexture();
        entity1.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);
        entity3.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);


        RenderingContext2D.When(x => x.DrawSpriteBatch(Arg.Any<SpriteBatch>())).Do(x =>
        {
            // Assert
            var spriteBatch = x.Arg<SpriteBatch>();
            Assert.That(spriteBatch.Count, Is.EqualTo(2));
            Assert.That(spriteBatch.Texture, Is.EqualTo(texture));

            var sprites = spriteBatch.GetSpritesSpan().ToArray();
            Assert.That(sprites, Has.One.Matches<SpriteBatchElement>(s => s.Sprite == entity1.GetSprite()));
            Assert.That(sprites, Has.One.Matches<SpriteBatchElement>(s => s.Sprite == entity3.GetSprite()));
        });

        // Act
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSpriteBatch(Arg.Any<SpriteBatch>());
        RenderingContext2D.Received(1).DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
    }

    [Test]
    public void RenderScene_ShouldDrawSpriteBatchAndDrawAnotherSpriteBatch_WhenSceneContainsTwoSpritesWithTheSameTextureAndTwoSpritesWithOtherTexture()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();

        var entity1 = renderingScene.AddSprite();
        var entity2 = renderingScene.AddSprite();
        var entity3 = renderingScene.AddSprite();
        var entity4 = renderingScene.AddSprite();

        var texture1 = CreateTexture();
        var texture2 = CreateTexture();
        entity1.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture1);
        entity2.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture2);
        entity3.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture1);
        entity4.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture2);


        RenderingContext2D.When(x => x.DrawSpriteBatch(Arg.Any<SpriteBatch>())).Do(x =>
        {
            // Assert
            var spriteBatch = x.Arg<SpriteBatch>();

            if (ReferenceEquals(spriteBatch.Texture, texture1))
            {
                Assert.That(spriteBatch.Count, Is.EqualTo(2));
                Assert.That(spriteBatch.Texture, Is.EqualTo(texture1));

                var sprites = spriteBatch.GetSpritesSpan().ToArray();
                Assert.That(sprites, Has.One.Matches<SpriteBatchElement>(s => s.Sprite == entity1.GetSprite()));
                Assert.That(sprites, Has.One.Matches<SpriteBatchElement>(s => s.Sprite == entity3.GetSprite()));
            }
            else if (ReferenceEquals(spriteBatch.Texture, texture2))
            {
                Assert.That(spriteBatch.Count, Is.EqualTo(2));
                Assert.That(spriteBatch.Texture, Is.EqualTo(texture2));

                var sprites = spriteBatch.GetSpritesSpan().ToArray();
                Assert.That(sprites, Has.One.Matches<SpriteBatchElement>(s => s.Sprite == entity2.GetSprite()));
                Assert.That(sprites, Has.One.Matches<SpriteBatchElement>(s => s.Sprite == entity4.GetSprite()));
            }
            else
            {
                Assert.Fail("Unexpected call to DrawSpriteBatch.");
            }
        });

        // Act
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(2).DrawSpriteBatch(Arg.Any<SpriteBatch>());
    }

    [Test]
    public void RenderScene_ShouldNotDrawSpriteBatch_WhenOrderInLayerPreventsSpriteBatching()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();
        renderingScene.AddCamera();

        var entity1 = renderingScene.AddSprite(orderInLayer: 0);
        var entity2 = renderingScene.AddSprite(orderInLayer: 1);
        var entity3 = renderingScene.AddSprite(orderInLayer: 2);

        var texture = CreateTexture();
        entity1.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);
        entity3.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);

        // Act
        renderingSystem.RenderScene();

        // Assert
        RenderingContext2D.DidNotReceive().DrawSpriteBatch(Arg.Any<SpriteBatch>());
        RenderingContext2D.Received(1).DrawSprite(entity1.GetSprite(), entity1.Get2DTransformationMatrix(), entity1.GetOpacity());
        RenderingContext2D.Received(1).DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
        RenderingContext2D.Received(1).DrawSprite(entity3.GetSprite(), entity3.Get2DTransformationMatrix(), entity3.GetOpacity());
    }

    [Test]
    public void SpriteRendererComponent_BoundingRectangle_ShouldReturnDefaultValue_WhenRenderingSystemIsNotAddedToSceneObservers()
    {
        // Arrange
        var (renderingSystem, renderingScene) = GetRenderingSystem();

        var entity = renderingScene.AddSprite(new Vector2(100, 200), new Vector2(10, 20), 0, new Vector2(2, 2));
        var spriteRendererComponent = entity.GetComponent<SpriteRendererComponent>();
        renderingScene.Scene.RemoveObserver(renderingSystem);

        // Act
        var actual = spriteRendererComponent.BoundingRectangle;

        // Assert
        Assert.That(spriteRendererComponent.IsManagedByRenderingSystem, Is.False);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle()));
    }

    [Test]
    public void SpriteRendererComponent_BoundingRectangle_ShouldReturnComputedValue_WhenRenderingSystemIsAddedToSceneObservers()
    {
        // Arrange
        var (_, renderingScene) = GetRenderingSystem();
        var entity = renderingScene.AddSprite(new Vector2(100, 200), new Vector2(10, 20), 0, new Vector2(2, 2));
        var spriteRendererComponent = entity.GetComponent<SpriteRendererComponent>();

        // Act
        var actual = spriteRendererComponent.BoundingRectangle;

        // Assert
        Assert.That(spriteRendererComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle(10, 20, 200, 400)));
    }
}