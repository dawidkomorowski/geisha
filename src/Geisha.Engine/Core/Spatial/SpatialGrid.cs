using System;
using System.Collections;
using System.Collections.Generic;
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

public interface IPairsQueryHandler
{
    bool Handle(SpatialGridProxyId proxyId1, SpatialGridProxyId proxyId2);
}

public sealed class SpatialGrid<TPayload> where TPayload : unmanaged
{
    private const int Null = -1;

    // Proxies
    private struct Proxy<T> : IUnmanaged<Proxy<T>> where T : unmanaged
    {
        public int Version;
        public int NextFreeIndex;

        public AABB2D Bounds;
        public T Payload;
    }

    private Proxy<TPayload>[] _proxies;
    private int _proxyFreeListHead;

    public readonly record struct ProxyData<T>
    {
        public AABB2D Bounds { get; init; }
        public T Payload { get; init; }
    }

    // Cells
    private struct CellNode : IUnmanaged<CellNode>
    {
        public int NextFreeIndex;
    }

    private CellNode[] _nodes;
    private int _nodeFreeListHead;

    // TODO: Describe how the cells are modelled.
    private Dictionary<long, int> _cells;

    public SpatialGrid(double cellSize) : this(new SizeD(cellSize, cellSize))
    {
    }

    public SpatialGrid(double cellSize, int capacity) : this(new SizeD(cellSize, cellSize), capacity)
    {
    }

    public SpatialGrid(SizeD cellSize) : this(cellSize, 4)
    {
    }

    public SpatialGrid(SizeD cellSize, int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentException("Capacity cannot be negative.");
        }

        CellSize = cellSize;

        // Proxies
        _proxies = new Proxy<TPayload>[capacity];
        _proxyFreeListHead = Null;

        for (var i = 0; i < _proxies.Length; i++)
        {
            _proxies[i].NextFreeIndex = i + 1;
        }

        if (_proxies.Length > 0)
        {
            _proxies[^1].NextFreeIndex = Null;
            _proxyFreeListHead = 0;
        }

        // Cells
        _nodes = new CellNode[capacity];
        _nodeFreeListHead = Null;

        for (var i = 0; i < _nodes.Length; i++)
        {
            _nodes[i].NextFreeIndex = i + 1;
        }

        if (_nodes.Length > 0)
        {
            _nodes[^1].NextFreeIndex = Null;
            _nodeFreeListHead = 0;
        }

        _cells = new Dictionary<long, int>(capacity);
    }

    public SizeD CellSize { get; }

    public bool IsValidProxy(SpatialGridProxyId id) => id.IsNotNull && _proxies[id.Index].Version == id.Version;

    public SpatialGridProxyId CreateProxy(in AABB2D bounds, TPayload payload)
    {
        if (_proxyFreeListHead == Null)
        {
            throw new NotImplementedException("Proxy array reallocation is not yet supported.");
        }

        var index = _proxyFreeListHead;

        ref var proxy = ref _proxies[index];
        proxy.Version++;
        proxy.Bounds = bounds;
        proxy.Payload = payload;

        _proxyFreeListHead = proxy.NextFreeIndex;

        return new SpatialGridProxyId(index, proxy.Version);
    }

    public void DestroyProxy(SpatialGridProxyId id)
    {
        ThrowIfInvalidId(id);

        ref var proxy = ref _proxies[id.Index];
        proxy.Version++;
        proxy.Bounds = default;
        proxy.Payload = default;
        proxy.NextFreeIndex = _proxyFreeListHead;

        _proxyFreeListHead = id.Index;
    }

    public ProxyData<TPayload> GetProxyData(SpatialGridProxyId id)
    {
        ThrowIfInvalidId(id);

        ref var proxy = ref _proxies[id.Index];

        return new ProxyData<TPayload>
        {
            Bounds = proxy.Bounds,
            Payload = proxy.Payload
        };
    }

    public void MoveProxy(SpatialGridProxyId id, in AABB2D newBounds)
    {
        ThrowIfInvalidId(id);

        ref var proxy = ref _proxies[id.Index];
        proxy.Bounds = newBounds;
    }

    public void QueryOverlappingPairs<TQueryHandler>(ref TQueryHandler handler) where TQueryHandler : struct, IPairsQueryHandler
    {
        // TODO: To be implemented.
    }

    private void ThrowIfInvalidId(SpatialGridProxyId id)
    {
        if (!IsValidProxy(id))
        {
            throw new InvalidOperationException("Invalid proxy id.");
        }
    }

    private static long BuildCellKey(int x, int y) => (long)x << 32 | (uint)y;

    private readonly record struct Cell(int X, int Y, long Key);

    private readonly record struct Cells(int MinX, int MinY, int MaxX, int MaxY) : IEnumerable<Cell>
    {
        public IEnumerator<Cell> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    // TODO: Temporary allocating implementation. Once logic is working it should be non allocating.
    //       Possible approaches:
    //          - use struct enumerator pattern
    //          - return cell bounds that allow iteration
    private Cells FindCells(AABB2D bounds)
    {
        var cellMinX = (int)System.Math.Floor(bounds.Min.X / CellSize.Width);
        var cellMinY = (int)System.Math.Floor(bounds.Min.Y / CellSize.Height);
        var cellMaxX = (int)System.Math.Ceiling(bounds.Max.X / CellSize.Width);
        var cellMaxY = (int)System.Math.Ceiling(bounds.Max.Y / CellSize.Height);

        return new Cells(cellMinX, cellMinY, cellMaxX, cellMaxY);
    }
}