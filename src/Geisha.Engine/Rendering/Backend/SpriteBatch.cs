using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Rendering.Backend;

// TODO Add documentation.
public sealed class SpriteBatch
{
    private readonly List<SpriteBatchElement> _sprites = new();

    public int Count => _sprites.Count;
    public ITexture? Texture => _sprites.Count == 0 ? null : _sprites[0].Sprite.SourceTexture;

    public void AddSprite(Sprite sprite, in Matrix3x3 transform, double opacity)
    {
        Debug.Assert(Count == 0 || sprite.SourceTexture == Texture, "Count == 0 || sprite.SourceTexture == Texture");

        _sprites.Add(new SpriteBatchElement(sprite, transform, opacity));
    }

    public void Clear()
    {
        _sprites.Clear();
    }

    public Span<SpriteBatchElement> GetSpanAccess()
    {
        return CollectionsMarshal.AsSpan(_sprites);
    }
}

public readonly struct SpriteBatchElement
{
    public SpriteBatchElement(Sprite sprite, in Matrix3x3 transform, double opacity)
    {
        Sprite = sprite;
        Transform = transform;
        Opacity = opacity;
    }

    public Sprite Sprite { get; }
    public Matrix3x3 Transform { get; }
    public double Opacity { get; }
}