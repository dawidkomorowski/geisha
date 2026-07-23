using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
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

public interface IProxyQueryHandler
{
    bool Handle(SpatialGridProxyId proxyId);
}

public interface IPairsQueryHandler
{
    bool Handle(SpatialGridProxyId proxyId1, SpatialGridProxyId proxyId2);
}

public sealed class SpatialGrid<TPayload> where TPayload : unmanaged
{
    private const int Null = -1;
    private const int DefaultCapacity = 4;

    // TODO: Describe what it is and why it is used.
    private int _queryId;

    private static long BuildCellKey(int x, int y) => (long)x << 32 | (uint)y;

    // TODO: Describe how the cells are modelled.
    private readonly Dictionary<long, int> _cells;

    // Proxies
    private struct Proxy<T> : IUnmanaged<Proxy<T>> where T : unmanaged
    {
        public int Version;
        public int NextFreeIndex;
        public int NodeListHead;
        public int LastQueryId;

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

    // Nodes
    private struct Node : IUnmanaged<Node>
    {
        public int NextFreeIndex;

        public int NextCellNodeIndex;
        public int PrevCellNodeIndex;

        public int NextProxyNodeIndex;
        public int PrevProxyNodeIndex;

        public int ProxyIndex;
        public long CellKey;

        public void Clear()
        {
            NextFreeIndex = Null;
            NextCellNodeIndex = Null;
            PrevCellNodeIndex = Null;
            NextProxyNodeIndex = Null;
            PrevProxyNodeIndex = Null;
            ProxyIndex = Null;
            CellKey = 0;
        }
    }

    private Node[] _nodes;
    private int _nodeFreeListHead;

    public SpatialGrid(double cellSize) : this(new SizeD(cellSize, cellSize))
    {
    }

    public SpatialGrid(double cellSize, int capacity) : this(new SizeD(cellSize, cellSize), capacity)
    {
    }

    public SpatialGrid(SizeD cellSize) : this(cellSize, DefaultCapacity)
    {
    }

    public SpatialGrid(SizeD cellSize, int capacity)
    {
        if (capacity < 0)
        {
            throw new ArgumentException("Capacity cannot be negative.");
        }

        CellSize = cellSize;
        _cells = new Dictionary<long, int>(capacity);

        _proxies = Array.Empty<Proxy<TPayload>>();
        _proxyFreeListHead = Null;

        _nodes = Array.Empty<Node>();
        _nodeFreeListHead = Null;

        if (capacity > 0)
        {
            GrowProxyPool(capacity);
            GrowNodePool(capacity);
        }
    }

    public SizeD CellSize { get; }

    public bool IsValidProxy(SpatialGridProxyId id) => id.IsNotNull && _proxies[id.Index].Version == id.Version;

    public SpatialGridProxyId CreateProxy(in AABB2D bounds, TPayload payload)
    {
        if (_proxyFreeListHead == Null)
        {
            GrowProxyPool(_proxies.Length + 1);
        }

        var index = _proxyFreeListHead;

        ref var proxy = ref _proxies[index];
        _proxyFreeListHead = proxy.NextFreeIndex;

        proxy.Version++;
        proxy.Bounds = bounds;
        proxy.Payload = payload;

        foreach (var cell in FindCells(bounds))
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

        while (proxy.NodeListHead != Null)
        {
            DestroyNode(proxy.NodeListHead);
        }

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

        while (proxy.NodeListHead != Null)
        {
            DestroyNode(proxy.NodeListHead);
        }

        foreach (var cell in FindCells(newBounds))
        {
            CreateNode(cell, ref proxy, id.Index);
        }
    }

    public void QueryPoint<TQueryHandler>(in Vector2 point, ref TQueryHandler handler) where TQueryHandler : struct, IProxyQueryHandler
    {
        var cell = FindCell(point);
        var nodeIndex = _cells.GetValueOrDefault(cell.Key, Null);

        var shouldContinue = true;
        while (nodeIndex != Null && shouldContinue)
        {
            ref var node = ref _nodes[nodeIndex];
            ref var proxy = ref _proxies[node.ProxyIndex];

            if (proxy.Bounds.Contains(point))
            {
                var proxyId = new SpatialGridProxyId(node.ProxyIndex, proxy.Version);
                shouldContinue = handler.Handle(proxyId);
            }

            nodeIndex = node.NextCellNodeIndex;
        }
    }

    public void QueryBounds<TQueryHandler>(in AABB2D bounds, ref TQueryHandler handler) where TQueryHandler : struct, IProxyQueryHandler
    {
        _queryId++;

        var shouldContinue = true;

        foreach (var cell in FindCells(bounds))
        {
            if (!shouldContinue)
            {
                break;
            }

            var nodeIndex = _cells.GetValueOrDefault(cell.Key, Null);
            while (nodeIndex != Null && shouldContinue)
            {
                ref var node = ref _nodes[nodeIndex];
                ref var proxy = ref _proxies[node.ProxyIndex];

                if (proxy.LastQueryId != _queryId)
                {
                    proxy.LastQueryId = _queryId;

                    if (proxy.Bounds.Overlaps(bounds))
                    {
                        var proxyId = new SpatialGridProxyId(node.ProxyIndex, proxy.Version);
                        shouldContinue = handler.Handle(proxyId);
                    }
                }

                nodeIndex = node.NextCellNodeIndex;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void QueryOverlappingPairs<TQueryHandler>(ref TQueryHandler handler) where TQueryHandler : struct, IPairsQueryHandler
    {
        var shouldContinue = true;

        foreach (var cell in _cells)
        {
            if (!shouldContinue)
            {
                break;
            }

            var node1Index = cell.Value;
            while (node1Index != Null && shouldContinue)
            {
                ref var node1 = ref _nodes[node1Index];

                var node2Index = node1.NextCellNodeIndex;
                while (node2Index != Null && shouldContinue)
                {
                    ref var node2 = ref _nodes[node2Index];

                    ref var proxy1 = ref _proxies[node1.ProxyIndex];
                    ref var proxy2 = ref _proxies[node2.ProxyIndex];

                    var intersection = proxy1.Bounds.Intersect(proxy2.Bounds);
                    if (intersection.IsValid)
                    {
                        var canonicalCell = FindCell(intersection.Min);

                        // Pair must be handled only in single canonical cell to avoid duplicates.
                        if (cell.Key == canonicalCell.Key)
                        {
                            var proxyId1 = new SpatialGridProxyId(node1.ProxyIndex, proxy1.Version);
                            var proxyId2 = new SpatialGridProxyId(node2.ProxyIndex, proxy2.Version);

                            shouldContinue = handler.Handle(proxyId1, proxyId2);
                        }
                    }

                    node2Index = node2.NextCellNodeIndex;
                }

                node1Index = node1.NextCellNodeIndex;
            }
        }
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
            GrowNodePool(_nodes.Length + 1);
        }

        var nodeIndex = _nodeFreeListHead;
        var cellListHead = _cells.GetValueOrDefault(cell.Key, Null);
        _cells[cell.Key] = nodeIndex;

        ref var node = ref _nodes[nodeIndex];
        _nodeFreeListHead = node.NextFreeIndex;

        node.NextCellNodeIndex = cellListHead;
        node.PrevCellNodeIndex = Null;

        if (node.NextCellNodeIndex != Null)
        {
            _nodes[node.NextCellNodeIndex].PrevCellNodeIndex = nodeIndex;
        }

        node.NextProxyNodeIndex = proxy.NodeListHead;
        node.PrevProxyNodeIndex = Null;
        proxy.NodeListHead = nodeIndex;

        if (node.NextProxyNodeIndex != Null)
        {
            _nodes[node.NextProxyNodeIndex].PrevProxyNodeIndex = nodeIndex;
        }

        node.ProxyIndex = proxyIndex;
        node.CellKey = cell.Key;
    }

    private void DestroyNode(int nodeIndex)
    {
        ref var node = ref _nodes[nodeIndex];

        if (node.NextCellNodeIndex != Null)
        {
            _nodes[node.NextCellNodeIndex].PrevCellNodeIndex = node.PrevCellNodeIndex;
        }

        if (node.PrevCellNodeIndex != Null)
        {
            _nodes[node.PrevCellNodeIndex].NextCellNodeIndex = node.NextCellNodeIndex;
        }

        if (_cells[node.CellKey] == nodeIndex)
        {
            if (node.NextCellNodeIndex == Null)
            {
                _cells.Remove(node.CellKey);
            }
            else
            {
                _cells[node.CellKey] = node.NextCellNodeIndex;
            }
        }

        if (node.NextProxyNodeIndex != Null)
        {
            _nodes[node.NextProxyNodeIndex].PrevProxyNodeIndex = node.PrevProxyNodeIndex;
        }

        if (node.PrevProxyNodeIndex != Null)
        {
            _nodes[node.PrevProxyNodeIndex].NextProxyNodeIndex = node.NextProxyNodeIndex;
        }

        ref var proxy = ref _proxies[node.ProxyIndex];
        if (proxy.NodeListHead == nodeIndex)
        {
            proxy.NodeListHead = node.NextProxyNodeIndex;
        }

        node.Clear();
        node.NextFreeIndex = _nodeFreeListHead;
        _nodeFreeListHead = nodeIndex;
    }

    private void GrowProxyPool(int capacity)
    {
        var oldCapacity = _proxies.Length;
        Debug.Assert(capacity > oldCapacity);

        GrowArrayExp(ref _proxies, capacity, DefaultCapacity);

        for (var i = oldCapacity; i < _proxies.Length; i++)
        {
            _proxies[i].NextFreeIndex = i + 1;
            _proxies[i].NodeListHead = Null;
        }

        _proxies[^1].NextFreeIndex = _proxyFreeListHead;
        _proxyFreeListHead = oldCapacity;
    }

    private void GrowNodePool(int capacity)
    {
        var oldCapacity = _nodes.Length;
        Debug.Assert(capacity > oldCapacity);

        GrowArrayExp(ref _nodes, capacity, DefaultCapacity);

        for (var i = oldCapacity; i < _nodes.Length; i++)
        {
            ref var node = ref _nodes[i];
            node.Clear();
            node.NextFreeIndex = i + 1;
        }

        _nodes[^1].NextFreeIndex = _nodeFreeListHead;
        _nodeFreeListHead = oldCapacity;
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

                return y <= _cellRange.MaxY;
            }

            public Cell Current { get; private set; }
        }
    }

    private CellRange FindCells(in AABB2D bounds)
    {
        var cellMinX = (int)System.Math.Floor(bounds.Min.X / CellSize.Width);
        var cellMinY = (int)System.Math.Floor(bounds.Min.Y / CellSize.Height);
        var cellMaxX = (int)System.Math.Floor(bounds.Max.X / CellSize.Width);
        var cellMaxY = (int)System.Math.Floor(bounds.Max.Y / CellSize.Height);

        return new CellRange(cellMinX, cellMinY, cellMaxX, cellMaxY);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Cell FindCell(in Vector2 point)
    {
        var x = (int)System.Math.Floor(point.X / CellSize.Width);
        var y = (int)System.Math.Floor(point.Y / CellSize.Height);

        return new Cell(x, y);
    }

    // TODO: This might be useful helper in other places. If so, move to ArrayEx?
    private static void GrowArrayExp<T>(ref T[] array, int minimumLength, int defaultLength)
    {
        Debug.Assert(minimumLength > array.Length);

        var newLength = array.Length == 0 ? defaultLength : 2 * array.Length;

        // Allow the array to grow to maximum possible length (~2G elements) before encountering overflow.
        // Note that this check works even when `array.Length` overflowed thanks to the (uint) cast.
        if ((uint)newLength > Array.MaxLength) newLength = Array.MaxLength;

        // If the computed length is still less than specified, set to the original argument.
        // Lengths exceeding Array.MaxLength will be surfaced as OutOfMemoryException by Array.Resize.
        if (newLength < minimumLength) newLength = minimumLength;

        Array.Resize(ref array, newLength);
    }
}