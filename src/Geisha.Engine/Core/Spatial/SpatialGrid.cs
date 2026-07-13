using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Memory;

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
    private const int Null = -1;
    private readonly List<GridObject<TPayload>> _gridObjects;
    private int _objectsFreeListHead;

    public SpatialGrid(double cellSize) : this(new SizeD(cellSize, cellSize))
    {
    }

    public SpatialGrid(SizeD cellSize)
    {
        CellSize = cellSize;

        _gridObjects = new List<GridObject<TPayload>>();
        _objectsFreeListHead = Null;
    }

    public SizeD CellSize { get; }

    public bool IsValidProxy(SpatialGridProxyId id) => id.IsNotNull && _gridObjects[id.Index].Version == id.Version;

    public SpatialGridProxyId CreateProxy(in AABB2D bounds, TPayload payload)
    {
        SpatialGridProxyId proxyId;

        if (_objectsFreeListHead == Null)
        {
            _gridObjects.Add(default);
            proxyId = new SpatialGridProxyId(_gridObjects.Capacity - 1, 1);
        }
        else
        {
            // TODO.
            proxyId = new SpatialGridProxyId(_gridObjects.Capacity - 1, 1);
        }

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

    private Span<GridObject<TPayload>> GetObjectsAsSpan() => CollectionsMarshal.AsSpan(_gridObjects);

    private struct GridObject<T> : IUnmanaged<GridObject<T>> where T : unmanaged
    {
        public int Version;
        public int NextFreeIndex;

        public AABB2D Bounds;
        public T Payload;
    }
}