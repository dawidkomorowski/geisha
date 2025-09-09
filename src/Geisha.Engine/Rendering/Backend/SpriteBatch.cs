using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Rendering.Backend;

/// <summary>
///     Represents a batch of sprites to render in single draw call.
/// </summary>
public sealed class SpriteBatch
{
    private readonly List<SpriteBatchElement> _sprites = new();

    /// <summary>
    ///     Returns <c>true</c> if <see cref="SpriteBatch" /> contains no sprites; otherwise <c>false</c>.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Texture))]
    public bool IsEmpty => _sprites.Count == 0;

    /// <summary>
    /// Gets the number of sprites contained in the <see cref="SpriteBatch"/>.
    /// </summary>
    public int Count => _sprites.Count;

    /// <summary>
    ///     Texture to use for drawing <see cref="SpriteBatch" />. It is derived from first <see cref="Sprite" /> added to
    ///     <see cref="SpriteBatch" />.
    /// </summary>
    public ITexture? Texture => _sprites.Count == 0 ? null : _sprites[0].Sprite.SourceTexture;

    /// <summary>
    ///     Adds a <paramref name="sprite" /> to the end of the <see cref="SpriteBatch" />.
    /// </summary>
    /// <param name="sprite"><see cref="Sprite" /> to add to the <see cref="SpriteBatch" />.</param>
    /// <param name="transform">Transformation of <see cref="Sprite" /> to apply when drawing.</param>
    /// <param name="opacity">Opacity of <see cref="Sprite" /> to apply when drawing.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown when <paramref name="sprite" /> is using texture different from sprites already present in the
    ///     <see cref="SpriteBatch" />.
    /// </exception>
    public void AddSprite(Sprite sprite, in Matrix3x3 transform, double opacity)
    {
        if (!IsEmpty && !ReferenceEquals(sprite.SourceTexture, Texture))
            throw new ArgumentException("Cannot add sprite using different texture than sprites already present in the batch.");

        _sprites.Add(new SpriteBatchElement(sprite, transform, opacity));
    }

    /// <summary>
    /// Removes all sprites from the <see cref="SpriteBatch"/>.
    /// </summary>
    public void Clear()
    {
        _sprites.Clear();
    }

    /// <summary>
    ///     Gets <see cref="Span{T}" /> view over sprites contained in the <see cref="SpriteBatch" />. The
    ///     <see cref="SpriteBatch" /> should not be modified while <see cref="Span{T}" /> is in use.
    /// </summary>
    /// <returns><see cref="Span{T}" /> view over sprites contained in the <see cref="SpriteBatch" />.</returns>
    public Span<SpriteBatchElement> GetSpritesSpan()
    {
        return CollectionsMarshal.AsSpan(_sprites);
    }
}

/// <summary>
///     Represents single <see cref="Rendering.Sprite" /> in the <see cref="SpriteBatch" />.
/// </summary>
public readonly struct SpriteBatchElement
{
    /// <summary>
    ///     Creates new instance of the <see cref="SpriteBatchElement" />.
    /// </summary>
    /// <param name="sprite">Sprite in the batch.</param>
    /// <param name="transform">Transformation of sprite.</param>
    /// <param name="opacity">Opacity of sprite.</param>
    public SpriteBatchElement(Sprite sprite, in Matrix3x3 transform, double opacity)
    {
        Sprite = sprite;
        Transform = transform;
        Opacity = opacity;
    }

    /// <summary>
    ///     Sprite in the batch.
    /// </summary>
    public Sprite Sprite { get; }

    /// <summary>
    ///     Transformation of sprite.
    /// </summary>
    public Matrix3x3 Transform { get; }

    /// <summary>
    ///     Opacity of sprite.
    /// </summary>
    public double Opacity { get; }
}