using System;
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
    private struct Node : IUnmanaged<Node>
    {
        public int NextFreeIndex;

        public int NextInCellIndex;
        public int PrevInCellIndex;

        public int NextOfProxyIndex;
        public int PrevOfProxyIndex;

        public int ProxyIndex;

        public void Clear()
        {
            NextFreeIndex = Null;
            NextInCellIndex = Null;
            PrevInCellIndex = Null;
            NextOfProxyIndex = Null;
            PrevOfProxyIndex = Null;
            ProxyIndex = Null;
        }
    }

    private Node[] _nodes;
    private int _nodeFreeListHead;

    // TODO: Describe how the cells are modelled.
    private Dictionary<long, int> _cells;
    private static long BuildCellKey(int x, int y) => (long)x << 32 | (uint)y;

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
        _nodes = new Node[capacity];
        _nodeFreeListHead = Null;

        for (var i = 0; i < _nodes.Length; i++)
        {
            ref var node = ref _nodes[i];
            node.Clear();
            node.NextFreeIndex = i + 1;
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
        _proxyFreeListHead = proxy.NextFreeIndex;

        proxy.Version++;
        proxy.Bounds = bounds;
        proxy.Payload = payload;

        var cellRange = FindCells(bounds);

        foreach (var cell in cellRange)
        {
            CreateNode(cell, ref proxy, index);
        }

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

    private void CreateNode(Cell cell, ref Proxy<TPayload> proxy, int proxyIndex)
    {
        if (_nodeFreeListHead == Null)
        {
            throw new NotImplementedException("Node array reallocation is not yet supported.");
        }

        var index = _nodeFreeListHead;
        var cellListHead = _cells.GetValueOrDefault(cell.Key, Null);
        _cells[cell.Key] = index;

        ref var node = ref _nodes[index];
        _nodeFreeListHead = node.NextFreeIndex;

        node.NextInCellIndex = cellListHead;
        node.PrevInCellIndex = Null;
        node.ProxyIndex = proxyIndex;
    }

    private readonly record struct Cell(int X, int Y)
    {
        public long Key { get; } = BuildCellKey(X, Y);
    }

    private readonly record struct CellRange(int MinX, int MinY, int MaxX, int MaxY)
    {
        // Minimal foreach support - simpler than full IEnumerable implementation.
        public Enumerator GetEnumerator() => new(this);

        public struct Enumerator
        {
            private readonly CellRange _cellRange;

            public Enumerator(CellRange cellRange)
            {
                _cellRange = cellRange;
                Current = new Cell(cellRange.MinX - 1, cellRange.MinY);
            }

            // MoveNext is called before first get of Current.
            public bool MoveNext()
            {
                var x = Current.X + 1;
                var y = Current.Y;

                if (x > _cellRange.MaxX)
                {
                    x = _cellRange.MinX;
                    y++;
                }

                Current = new Cell(x, y);

                return y <= _cellRange.MaxX;
            }

            public Cell Current { get; private set; }
        }
    }

    private CellRange FindCells(AABB2D bounds)
    {
        var cellMinX = (int)System.Math.Floor(bounds.Min.X / CellSize.Width);
        var cellMinY = (int)System.Math.Floor(bounds.Min.Y / CellSize.Height);
        var cellMaxX = (int)System.Math.Floor(bounds.Max.X / CellSize.Width);
        var cellMaxY = (int)System.Math.Floor(bounds.Max.Y / CellSize.Height);

        return new CellRange(cellMinX, cellMinY, cellMaxX, cellMaxY);
    }
}