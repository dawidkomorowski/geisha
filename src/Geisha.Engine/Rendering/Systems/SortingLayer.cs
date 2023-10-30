using System.Collections.Generic;

namespace Geisha.Engine.Rendering.Systems;

internal sealed class SortingLayer
{
    private readonly List<RenderNode> _renderNodes = new();

    public SortingLayer(string name)
    {
        Name = name;
    }

    public string Name { get; }

    public IReadOnlyList<RenderNode> GetRenderNodes() => _renderNodes;

    public void Add(RenderNode node)
    {
        _renderNodes.Add(node);
    }

    public void Remove(RenderNode node)
    {
        _renderNodes.Remove(node);
    }
}