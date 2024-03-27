using Geisha.Engine.Core;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering;
using Geisha.Engine.Rendering.Backend;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Backend;

[TestFixture]
public class SpriteBatchTests
{
    [Test]
    public void IsEmpty_ShouldBeTrue_ForNewSpriteBatch()
    {
        // Arrange
        // Act
        var spriteBatch = new SpriteBatch();

        // Assert
        Assert.That(spriteBatch.IsEmpty, Is.True);
    }

    [Test]
    public void IsEmpty_ShouldBeFalse_WhenSpriteAddedToBatch()
    {
        // Arrange
        var spriteBatch = new SpriteBatch();

        // Act
        spriteBatch.AddSprite(CreateSprite(CreateTexture()), Matrix3x3.Identity, 1);

        // Assert
        Assert.That(spriteBatch.IsEmpty, Is.False);
    }

    [Test]
    public void Count_ShouldBe3_When3SpritesAddedToBatch()
    {
        // Arrange
        var spriteBatch = new SpriteBatch();

        var texture = CreateTexture();
        var sprite1 = CreateSprite(texture);
        var sprite2 = CreateSprite(texture);
        var sprite3 = CreateSprite(texture);

        // Act
        spriteBatch.AddSprite(sprite1, Matrix3x3.Identity, 1);
        spriteBatch.AddSprite(sprite2, Matrix3x3.Identity, 1);
        spriteBatch.AddSprite(sprite3, Matrix3x3.Identity, 1);

        // Assert
        Assert.That(spriteBatch.Count, Is.EqualTo(3));
    }

    [Test]
    public void Texture_ShouldBeNull_ForNewSpriteBatch()
    {
        // Arrange
        // Act
        var spriteBatch = new SpriteBatch();

        // Assert
        Assert.That(spriteBatch.Texture, Is.Null);
    }

    [Test]
    public void Texture_ShouldBeDerivedFromSprite_WhenSpriteAddedToBatch()
    {
        // Arrange
        var spriteBatch = new SpriteBatch();
        var texture = CreateTexture();

        // Act
        spriteBatch.AddSprite(CreateSprite(texture), Matrix3x3.Identity, 1);

        // Assert
        Assert.That(spriteBatch.Texture, Is.EqualTo(texture));
    }

    [Test]
    public void AddSprite_ShouldThrowException_WhenSpritesWithDifferentTexturesAreAddedToBatch()
    {
        // Arrange
        var spriteBatch = new SpriteBatch();

        var texture1 = CreateTexture();
        var texture2 = CreateTexture();
        var sprite1 = CreateSprite(texture1);
        var sprite2 = CreateSprite(texture2);

        // Act
        spriteBatch.AddSprite(sprite1, Matrix3x3.Identity, 1);

        // Assert
        Assert.That(() => spriteBatch.AddSprite(sprite2, Matrix3x3.Identity, 1), Throws.ArgumentException);
    }

    [Test]
    public void Clear_ShouldRemoveAllSpritesFromBatch()
    {
        // Arrange
        var spriteBatch = new SpriteBatch();

        var texture = CreateTexture();

        spriteBatch.AddSprite(CreateSprite(texture), Matrix3x3.Identity, 1);
        spriteBatch.AddSprite(CreateSprite(texture), Matrix3x3.Identity, 1);
        spriteBatch.AddSprite(CreateSprite(texture), Matrix3x3.Identity, 1);

        Assert.That(spriteBatch.Count, Is.EqualTo(3));

        // Act
        spriteBatch.Clear();

        // Assert
        Assert.That(spriteBatch.Count, Is.EqualTo(0));
        Assert.That(spriteBatch.IsEmpty, Is.True);
    }

    [Test]
    public void GetSpanAccess_ShouldReturnSpanThatGivesAccessToSpritesAddedToBatch()
    {
        // Arrange
        var spriteBatch = new SpriteBatch();

        var texture = CreateTexture();
        var sprite1 = CreateSprite(texture);
        var sprite2 = CreateSprite(texture);
        var sprite3 = CreateSprite(texture);

        spriteBatch.AddSprite(sprite1, Matrix3x3.CreateRotation(1), 0.1);
        spriteBatch.AddSprite(sprite2, Matrix3x3.CreateRotation(2), 0.2);
        spriteBatch.AddSprite(sprite3, Matrix3x3.CreateRotation(3), 0.3);

        // Act
        var spanOfSprites = spriteBatch.GetSpanAccess();

        // Assert
        Assert.That(spanOfSprites.Length, Is.EqualTo(3));

        Assert.That(spanOfSprites[0].Sprite, Is.EqualTo(sprite1));
        Assert.That(spanOfSprites[0].Transform, Is.EqualTo(Matrix3x3.CreateRotation(1)));
        Assert.That(spanOfSprites[0].Opacity, Is.EqualTo(0.1));

        Assert.That(spanOfSprites[1].Sprite, Is.EqualTo(sprite2));
        Assert.That(spanOfSprites[1].Transform, Is.EqualTo(Matrix3x3.CreateRotation(2)));
        Assert.That(spanOfSprites[1].Opacity, Is.EqualTo(0.2));

        Assert.That(spanOfSprites[2].Sprite, Is.EqualTo(sprite3));
        Assert.That(spanOfSprites[2].Transform, Is.EqualTo(Matrix3x3.CreateRotation(3)));
        Assert.That(spanOfSprites[2].Opacity, Is.EqualTo(0.3));
    }

    private static ITexture CreateTexture()
    {
        var texture = Substitute.For<ITexture>();
        texture.RuntimeId.Returns(RuntimeId.Next());
        return texture;
    }

    private static Sprite CreateSprite(ITexture texture)
    {
        return new Sprite(texture, Vector2.Zero, new Vector2(10, 10), Vector2.Zero, 1);
    }
}