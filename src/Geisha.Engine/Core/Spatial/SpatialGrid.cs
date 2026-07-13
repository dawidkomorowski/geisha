using System;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Core.Spatial;

public readonly record struct SpatialGridProxyId
{
    private readonly int _value;

    public static SpatialGridProxyId Null => default;

    public SpatialGridProxyId(int index, int version)
    {
        _value = index + 1;
        Version = version;
    }

    public int Index => _value - 1;
    public int Version { get; }

    public bool IsNull => !IsNotNull;
    public bool IsNotNull => _value > 0;
}

// TODO: Scaffolding for spatial grid prototyping.
public sealed class SpatialGrid<TPayload> where TPayload : unmanaged
{
    public SpatialGrid(double cellSize) : this(new SizeD(cellSize, cellSize))
    {
    }

    public SpatialGrid(SizeD cellSize)
    {
        CellSize = cellSize;
    }

    public SizeD CellSize { get; }

    public SpatialGridProxyId CreateProxy(in AABB2D bounds, TPayload payload)
    {
        return default;
    }

    public void MoveProxy(SpatialGridProxyId id, in AABB2D newBounds)
    {
        throw new NotImplementedException();
    }

    public void DestroyProxy(SpatialGridProxyId id)
    {
        throw new NotImplementedException();
    }
}