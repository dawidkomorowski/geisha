using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Geisha.Engine.Rendering.Systems;

internal sealed class SortingLayer
{
    private readonly List<RenderNode> _renderNodes = new();

    public SortingLayer(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public ReadOnlySpan<RenderNode> GetRenderNodesSpan() => CollectionsMarshal.AsSpan(_renderNodes);

    public void Add(RenderNode node)
    {
        _renderNodes.Add(node);
    }

    public void Remove(RenderNode node)
    {
        _renderNodes.Remove(node);
    }
}