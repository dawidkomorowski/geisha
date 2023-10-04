using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
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

        var (renderingSystem, renderingScene) = GetRenderingSystem();

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
        Assume.That(spriteRendererComponent.IsManagedByRenderingSystem, Is.True);

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

        var (renderingSystem, renderingScene) = GetRenderingSystem();
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
        Assume.That(spriteRendererComponent.IsManagedByRenderingSystem, Is.False);

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