using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using Geisha.Engine.Rendering.Components;
using NSubstitute;
using NUnit.Framework;
using System.Linq;

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

        var context = CreateRenderingTestContext(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, sortingLayerName } });

        var entity = context.AddSprite();
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
        context.Scene.RemoveObserver(context.RenderingSystem);

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

        var context = CreateRenderingTestContext(new RenderingConfiguration
            { SortingLayersOrder = new[] { RenderingConfiguration.DefaultSortingLayerName, sortingLayerName } });
        context.Scene.RemoveObserver(context.RenderingSystem);

        var entity = context.AddSprite();
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
        context.Scene.AddObserver(context.RenderingSystem);

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
        var context = CreateRenderingTestContext();
        context.AddCamera();
        var entity = context.AddSprite();

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(entity.GetSprite(), entity.Get2DTransformationMatrix(), entity.GetOpacity());
    }

    [Test]
    public void RenderScene_ShouldDrawSprite_WhenOpacityIsNonDefault()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();
        var entity = context.AddSprite();
        var spriteRendererComponent = entity.GetComponent<SpriteRendererComponent>();
        spriteRendererComponent.Opacity = 0.5;

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(entity.GetSprite(), entity.Get2DTransformationMatrix(), 0.5);
    }

    [Test]
    public void RenderScene_ShouldDrawSprite_TransformedWithParentTransform_WhenEntityHasParentWithTransform2DComponent()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();

        var parent = context.Scene.CreateEntity();
        var parentTransform = parent.CreateComponent<Transform2DComponent>();
        parentTransform.Translation = new Vector2(10, 20);
        parentTransform.Rotation = 30;
        parentTransform.Scale = new Vector2(2, 4);

        var child = context.AddSprite();
        child.Parent = parent;

        var expectedTransform = parentTransform.ToMatrix() * child.Get2DTransformationMatrix();

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(child.GetSprite(), expectedTransform);
    }

    [Test]
    public void RenderScene_ShouldDrawSprite_WhenTransformIsInterpolated()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();
        var entity = context.AddSprite(new Vector2(100, 200), new Vector2(10, 20), 30, new Vector2(1, 2));
        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        transform2DComponent.IsInterpolated = true;

        context.TransformInterpolationSystem.SnapshotTransforms();

        transform2DComponent.Translation = new Vector2(20, 40);
        transform2DComponent.Rotation = 60;
        transform2DComponent.Scale = new Vector2(2, 4);

        context.TransformInterpolationSystem.SnapshotTransforms();

        context.TransformInterpolationSystem.InterpolateTransforms(0.5);

        // Assume
        Assert.That(transform2DComponent.InterpolatedTransform, Is.Not.EqualTo(transform2DComponent.Transform));

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(entity.GetSprite(), transform2DComponent.InterpolatedTransform.ToMatrix());
    }

    [Test]
    public void RenderScene_ShouldDrawSprite_TransformedWithParentTransform_WhenTransformIsInterpolated()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();

        var parent = context.Scene.CreateEntity();
        var parentTransform = parent.CreateComponent<Transform2DComponent>();
        parentTransform.IsInterpolated = true;
        parentTransform.Translation = new Vector2(10, 20);
        parentTransform.Rotation = 2;
        parentTransform.Scale = new Vector2(1, 2);

        var child = context.AddSprite(new Vector2(100, 200), new Vector2(20, 30), 3, new Vector2(2, 3));
        child.Parent = parent;
        var childTransform = child.GetComponent<Transform2DComponent>();
        childTransform.IsInterpolated = true;

        context.TransformInterpolationSystem.SnapshotTransforms();

        parentTransform.Translation = new Vector2(20, 40);
        parentTransform.Rotation = 4;
        parentTransform.Scale = new Vector2(2, 4);

        childTransform.Translation = new Vector2(30, 60);
        childTransform.Rotation = 6;
        childTransform.Scale = new Vector2(3, 6);

        context.TransformInterpolationSystem.SnapshotTransforms();

        context.TransformInterpolationSystem.InterpolateTransforms(0.5);

        // Assume
        Assert.That(parentTransform.InterpolatedTransform, Is.Not.EqualTo(parentTransform.Transform));
        Assert.That(childTransform.InterpolatedTransform, Is.Not.EqualTo(childTransform.Transform));

        var expectedTransform = parentTransform.InterpolatedTransform.ToMatrix() * childTransform.InterpolatedTransform.ToMatrix();

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSprite(child.GetSprite(), expectedTransform);
    }

    [Test]
    public void RenderScene_ShouldDrawSpriteBatch_WhenSceneContainsTwoSpritesWithTheSameTexture()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();

        var entity1 = context.AddSprite(translation: new Vector2(10, 20), opacity: 1d);
        var entity2 = context.AddSprite(translation: new Vector2(30, 40), opacity: 0.5d);

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
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSpriteBatch(Arg.Any<SpriteBatch>());
    }

    [Test]
    public void RenderScene_ShouldDrawSpriteBatch_WithSpritesTransformedWithParentTransform_WhenEachEntityHasParentWithTransform2DComponent()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();

        var texture = CreateTexture();

        var parent1 = context.Scene.CreateEntity();
        var parent1Transform = parent1.CreateComponent<Transform2DComponent>();
        parent1Transform.Translation = new Vector2(1, 2);

        var parent2 = context.Scene.CreateEntity();
        var parent2Transform = parent2.CreateComponent<Transform2DComponent>();
        parent2Transform.Translation = new Vector2(3, 4);

        var child1 = context.AddSprite(translation: new Vector2(10, 20));
        child1.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);
        child1.Parent = parent1;

        var child2 = context.AddSprite(translation: new Vector2(30, 40));
        child2.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);
        child2.Parent = parent2;

        var expectedTransform1 = parent1Transform.ToMatrix() * child1.Get2DTransformationMatrix();
        var expectedTransform2 = parent2Transform.ToMatrix() * child2.Get2DTransformationMatrix();

        RenderingContext2D.When(x => x.DrawSpriteBatch(Arg.Any<SpriteBatch>())).Do(x =>
        {
            // Assert
            var spriteBatch = x.Arg<SpriteBatch>();
            Assert.That(spriteBatch.Count, Is.EqualTo(2));
            Assert.That(spriteBatch.Texture, Is.EqualTo(texture));

            var sprites = spriteBatch.GetSpritesSpan().ToArray();
            var spriteBatchElement1 = sprites.Single(s => s.Sprite == child1.GetSprite());
            var spriteBatchElement2 = sprites.Single(s => s.Sprite == child2.GetSprite());

            Assert.That(spriteBatchElement1.Transform, Is.EqualTo(expectedTransform1));
            Assert.That(spriteBatchElement1.Opacity, Is.EqualTo(child1.GetOpacity()));

            Assert.That(spriteBatchElement2.Transform, Is.EqualTo(expectedTransform2));
            Assert.That(spriteBatchElement2.Opacity, Is.EqualTo(child2.GetOpacity()));
        });

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSpriteBatch(Arg.Any<SpriteBatch>());
    }

    [Test]
    public void RenderScene_ShouldDrawSpriteBatch_WhenTransformIsInterpolated()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();

        var entity1 = context.AddSprite(translation: new Vector2(10, 20), opacity: 1d);
        var entity2 = context.AddSprite(translation: new Vector2(30, 40), opacity: 0.5d);

        var texture = CreateTexture();
        entity1.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);
        entity2.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);

        var transform2DComponent1 = entity1.GetComponent<Transform2DComponent>();
        transform2DComponent1.IsInterpolated = true;

        var transform2DComponent2 = entity2.GetComponent<Transform2DComponent>();
        transform2DComponent2.IsInterpolated = true;

        context.TransformInterpolationSystem.SnapshotTransforms();

        transform2DComponent1.Translation = new Vector2(20, 30);
        transform2DComponent2.Translation = new Vector2(40, 50);

        context.TransformInterpolationSystem.SnapshotTransforms();

        context.TransformInterpolationSystem.InterpolateTransforms(0.5);

        // Assume
        Assert.That(transform2DComponent1.InterpolatedTransform, Is.Not.EqualTo(transform2DComponent1.Transform));
        Assert.That(transform2DComponent2.InterpolatedTransform, Is.Not.EqualTo(transform2DComponent2.Transform));


        RenderingContext2D.When(x => x.DrawSpriteBatch(Arg.Any<SpriteBatch>())).Do(x =>
        {
            // Assert
            var spriteBatch = x.Arg<SpriteBatch>();
            Assert.That(spriteBatch.Count, Is.EqualTo(2));
            Assert.That(spriteBatch.Texture, Is.EqualTo(texture));

            var sprites = spriteBatch.GetSpritesSpan().ToArray();
            var spriteBatchElement1 = sprites.Single(s => s.Sprite == entity1.GetSprite());
            var spriteBatchElement2 = sprites.Single(s => s.Sprite == entity2.GetSprite());

            Assert.That(spriteBatchElement1.Transform, Is.EqualTo(transform2DComponent1.InterpolatedTransform.ToMatrix()));
            Assert.That(spriteBatchElement1.Opacity, Is.EqualTo(entity1.GetOpacity()));

            Assert.That(spriteBatchElement2.Transform, Is.EqualTo(transform2DComponent2.InterpolatedTransform.ToMatrix()));
            Assert.That(spriteBatchElement2.Opacity, Is.EqualTo(entity2.GetOpacity()));
        });

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSpriteBatch(Arg.Any<SpriteBatch>());
    }

    [Test]
    public void RenderScene_ShouldDrawSpriteBatch_WithSpritesTransformedWithParentTransform_WhenTransformIsInterpolated()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();

        var texture = CreateTexture();

        var parent1 = context.Scene.CreateEntity();
        var parent1Transform = parent1.CreateComponent<Transform2DComponent>();
        parent1Transform.Translation = new Vector2(1, 2);
        parent1Transform.IsInterpolated = true;

        var parent2 = context.Scene.CreateEntity();
        var parent2Transform = parent2.CreateComponent<Transform2DComponent>();
        parent2Transform.Translation = new Vector2(3, 4);
        parent2Transform.IsInterpolated = true;

        var child1 = context.AddSprite(translation: new Vector2(10, 20));
        child1.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);
        child1.Parent = parent1;
        var child1Transform = child1.GetComponent<Transform2DComponent>();
        child1Transform.IsInterpolated = true;

        var child2 = context.AddSprite(translation: new Vector2(30, 40));
        child2.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);
        child2.Parent = parent2;
        var child2Transform = child2.GetComponent<Transform2DComponent>();
        child2Transform.IsInterpolated = true;

        context.TransformInterpolationSystem.SnapshotTransforms();

        parent1Transform.Translation = new Vector2(2, 3);
        parent2Transform.Translation = new Vector2(4, 5);
        child1Transform.Translation = new Vector2(20, 30);
        child2Transform.Translation = new Vector2(40, 50);

        context.TransformInterpolationSystem.SnapshotTransforms();

        context.TransformInterpolationSystem.InterpolateTransforms(0.5);

        var expectedTransform1 = parent1Transform.InterpolatedTransform.ToMatrix() * child1Transform.InterpolatedTransform.ToMatrix();
        var expectedTransform2 = parent2Transform.InterpolatedTransform.ToMatrix() * child2Transform.InterpolatedTransform.ToMatrix();

        // Assume
        Assert.That(parent1Transform.InterpolatedTransform, Is.Not.EqualTo(parent1Transform.Transform));
        Assert.That(parent2Transform.InterpolatedTransform, Is.Not.EqualTo(parent2Transform.Transform));
        Assert.That(child1Transform.InterpolatedTransform, Is.Not.EqualTo(child1Transform.Transform));
        Assert.That(child2Transform.InterpolatedTransform, Is.Not.EqualTo(child2Transform.Transform));

        RenderingContext2D.When(x => x.DrawSpriteBatch(Arg.Any<SpriteBatch>())).Do(x =>
        {
            // Assert
            var spriteBatch = x.Arg<SpriteBatch>();
            Assert.That(spriteBatch.Count, Is.EqualTo(2));
            Assert.That(spriteBatch.Texture, Is.EqualTo(texture));

            var sprites = spriteBatch.GetSpritesSpan().ToArray();
            var spriteBatchElement1 = sprites.Single(s => s.Sprite == child1.GetSprite());
            var spriteBatchElement2 = sprites.Single(s => s.Sprite == child2.GetSprite());

            Assert.That(spriteBatchElement1.Transform, Is.EqualTo(expectedTransform1));
            Assert.That(spriteBatchElement1.Opacity, Is.EqualTo(child1.GetOpacity()));

            Assert.That(spriteBatchElement2.Transform, Is.EqualTo(expectedTransform2));
            Assert.That(spriteBatchElement2.Opacity, Is.EqualTo(child2.GetOpacity()));
        });

        // Act
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSpriteBatch(Arg.Any<SpriteBatch>());
    }

    [Test]
    public void RenderScene_ShouldDrawSpriteBatchAndDrawSprite_WhenSceneContainsTwoSpritesWithTheSameTextureAndOneSpriteWithDifferentTexture()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();

        var entity1 = context.AddSprite();
        var entity2 = context.AddSprite();
        var entity3 = context.AddSprite();

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
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(1).DrawSpriteBatch(Arg.Any<SpriteBatch>());
        RenderingContext2D.Received(1).DrawSprite(entity2.GetSprite(), entity2.Get2DTransformationMatrix(), entity2.GetOpacity());
    }

    [Test]
    public void RenderScene_ShouldDrawSpriteBatchAndDrawAnotherSpriteBatch_WhenSceneContainsTwoSpritesWithTheSameTextureAndTwoSpritesWithOtherTexture()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();

        var entity1 = context.AddSprite();
        var entity2 = context.AddSprite();
        var entity3 = context.AddSprite();
        var entity4 = context.AddSprite();

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
        context.RenderingSystem.RenderScene();

        // Assert
        RenderingContext2D.Received(2).DrawSpriteBatch(Arg.Any<SpriteBatch>());
    }

    [Test]
    public void RenderScene_ShouldNotDrawSpriteBatch_WhenOrderInLayerPreventsSpriteBatching()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        context.AddCamera();

        var entity1 = context.AddSprite(orderInLayer: 0);
        var entity2 = context.AddSprite(orderInLayer: 1);
        var entity3 = context.AddSprite(orderInLayer: 2);

        var texture = CreateTexture();
        entity1.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);
        entity3.GetComponent<SpriteRendererComponent>().Sprite = CreateSprite(texture);

        // Act
        context.RenderingSystem.RenderScene();

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
        var context = CreateRenderingTestContext();

        var entity = context.AddSprite(new Vector2(100, 200), new Vector2(10, 20), 0, new Vector2(2, 2));
        var spriteRendererComponent = entity.GetComponent<SpriteRendererComponent>();
        context.Scene.RemoveObserver(context.RenderingSystem);

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
        var context = CreateRenderingTestContext();
        var entity = context.AddSprite(new Vector2(100, 200), new Vector2(10, 20), 0, new Vector2(2, 2));
        var spriteRendererComponent = entity.GetComponent<SpriteRendererComponent>();

        // Act
        var actual = spriteRendererComponent.BoundingRectangle;

        // Assert
        Assert.That(spriteRendererComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle(10, 20, 200, 400)));
    }

    [Test]
    public void SpriteRendererComponent_BoundingRectangle_ShouldReturnComputedValue_WhenTransformIsInterpolated()
    {
        // Arrange
        var context = CreateRenderingTestContext();
        var entity = context.AddSprite(new Vector2(100, 200), new Vector2(10, 20), 0, new Vector2(2, 2));
        var spriteRendererComponent = entity.GetComponent<SpriteRendererComponent>();
        var transform2DComponent = entity.GetComponent<Transform2DComponent>();
        transform2DComponent.IsInterpolated = true;

        context.TransformInterpolationSystem.SnapshotTransforms();

        transform2DComponent.Translation = new Vector2(20, 40);
        transform2DComponent.Scale = new Vector2(4, 4);

        context.TransformInterpolationSystem.SnapshotTransforms();

        context.TransformInterpolationSystem.InterpolateTransforms(0.5);

        // Assume
        Assert.That(transform2DComponent.InterpolatedTransform, Is.Not.EqualTo(transform2DComponent.Transform));

        // Act
        var actual = spriteRendererComponent.BoundingRectangle;

        // Assert
        Assert.That(spriteRendererComponent.IsManagedByRenderingSystem, Is.True);
        Assert.That(actual, Is.EqualTo(new AxisAlignedRectangle(15, 30, 300, 600)));
    }
}