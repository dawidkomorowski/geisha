using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Memory;

namespace Geisha.Engine.Core.Spatial;

/// <summary>
/// Identifies a proxy stored in <see cref="SpatialGrid{TPayload}"/>.
/// </summary>
public readonly record struct SpatialGridProxyId
{
    private readonly int _value;

    /// <summary>
    /// Gets a null proxy identifier.
    /// </summary>
    public static SpatialGridProxyId Null => default;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpatialGridProxyId"/> struct.
    /// </summary>
    /// <param name="index">Index of proxy in internal proxy storage.</param>
    /// <param name="version">Version of proxy used to validate stale identifiers.</param>
    public SpatialGridProxyId(int index, int version)
    {
        _value = index + 1;
        Version = version;
    }

    /// <summary>
    /// Gets the index of proxy in internal proxy storage.
    /// </summary>
    public int Index => _value - 1;

    /// <summary>
    /// Gets the version of proxy in internal proxy storage.
    /// </summary>
    public int Version { get; }

    /// <summary>
    /// Gets a value indicating whether this identifier is null.
    /// </summary>
    public bool IsNull => !IsNotNull;

    /// <summary>
    /// Gets a value indicating whether this identifier is not null.
    /// </summary>
    public bool IsNotNull => _value > 0;
}

/// <summary>
/// Handles queries that return proxy identifiers.
/// </summary>
public interface IProxyIdQueryHandler
{
    /// <summary>
    /// Handles a single query result.
    /// </summary>
    /// <param name="proxyId">Identifier of proxy that matched query.</param>
    /// <returns><see langword="true"/> to continue query; otherwise, <see langword="false"/> to stop.</returns>
    bool Handle(SpatialGridProxyId proxyId);
}

/// <summary>
/// Handles queries that return proxy payloads.
/// </summary>
/// <typeparam name="TPayload">Type of payload stored in a proxy.</typeparam>
public interface IProxyPayloadQueryHandler<TPayload> where TPayload : unmanaged
{
    /// <summary>
    /// Handles a single query result.
    /// </summary>
    /// <param name="payload">Payload of proxy that matched query.</param>
    /// <returns><see langword="true"/> to continue query; otherwise, <see langword="false"/> to stop.</returns>
    bool Handle(in TPayload payload);
}

/// <summary>
/// Handles queries that return pairs of proxy identifiers.
/// </summary>
public interface IProxyIdPairQueryHandler
{
    /// <summary>
    /// Handles a single query result.
    /// </summary>
    /// <param name="proxyId1">Identifier of first proxy in a matching pair.</param>
    /// <param name="proxyId2">Identifier of second proxy in a matching pair.</param>
    /// <returns><see langword="true"/> to continue query; otherwise, <see langword="false"/> to stop.</returns>
    bool Handle(SpatialGridProxyId proxyId1, SpatialGridProxyId proxyId2);
}

/// <summary>
/// Handles queries that return pairs of proxy payloads.
/// </summary>
/// <typeparam name="TPayload">Type of payload stored in a proxy.</typeparam>
public interface IProxyPayloadPairQueryHandler<TPayload> where TPayload : unmanaged
{
    /// <summary>
    /// Handles a single query result.
    /// </summary>
    /// <param name="payload1">Payload of first proxy in a matching pair.</param>
    /// <param name="payload2">Payload of second proxy in a matching pair.</param>
    /// <returns><see langword="true"/> to continue query; otherwise, <see langword="false"/> to stop.</returns>
    bool Handle(in TPayload payload1, in TPayload payload2);
}

/// <summary>
/// Spatial index that maps proxies to uniform grid cells for efficient point, bounds, and overlap queries.
/// </summary>
/// <typeparam name="TPayload">Type of payload stored in a proxy.</typeparam>
public sealed class SpatialGrid<TPayload> where TPayload : unmanaged
{
    private const int Null = -1;
    private const int DefaultCapacity = 4;

    // Monotonic token identifying current bounds query; compared with Proxy.LastQueryId so each proxy
    // is processed at most once even when it appears in multiple visited cells.
    private int _queryId;

    private static long BuildCellKey(int x, int y) => (long)x << 32 | (uint)y;

    // Sparse uniform grid: each occupied cell is identified by packed (x, y) key and maps to
    // the head node index of an intrusive linked list of proxies intersecting that cell.
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

    /// <summary>
    /// Data stored in a proxy.
    /// </summary>
    /// <typeparam name="T">Type of payload.</typeparam>
    public readonly record struct ProxyData<T>
    {
        /// <summary>
        /// Gets axis-aligned bounding box of proxy.
        /// </summary>
        public AABB2D Bounds { get; init; }

        /// <summary>
        /// Gets payload of proxy.
        /// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="SpatialGrid{TPayload}"/> class using square cells.
    /// </summary>
    /// <param name="cellSize">Width and height of each cell.</param>
    public SpatialGrid(double cellSize) : this(new SizeD(cellSize, cellSize))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpatialGrid{TPayload}"/> class using square cells and initial capacity.
    /// </summary>
    /// <param name="cellSize">Width and height of each cell.</param>
    /// <param name="capacity">Initial capacity for internal sparse storage.</param>
    /// <exception cref="ArgumentException"><paramref name="capacity"/> is negative.</exception>
    public SpatialGrid(double cellSize, int capacity) : this(new SizeD(cellSize, cellSize), capacity)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpatialGrid{TPayload}"/> class.
    /// </summary>
    /// <param name="cellSize">Size of each cell.</param>
    public SpatialGrid(SizeD cellSize) : this(cellSize, DefaultCapacity)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SpatialGrid{TPayload}"/> class.
    /// </summary>
    /// <param name="cellSize">Size of each cell.</param>
    /// <param name="capacity">Initial capacity for internal sparse storage.</param>
    /// <exception cref="ArgumentException"><paramref name="capacity"/> is negative.</exception>
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

    /// <summary>
    /// Gets size of each grid cell.
    /// </summary>
    public SizeD CellSize { get; }

    /// <summary>
    /// Determines whether specified proxy identifier points to a currently valid proxy.
    /// </summary>
    /// <param name="id">Proxy identifier to validate.</param>
    /// <returns><see langword="true"/> if identifier is valid; otherwise, <see langword="false"/>.</returns>
    public bool IsValidProxy(SpatialGridProxyId id) => id.IsNotNull && _proxies[id.Index].Version == id.Version;

    /// <summary>
    /// Creates a new proxy and inserts it into all cells overlapped by provided bounds.
    /// </summary>
    /// <param name="bounds">Axis-aligned bounding box of proxy.</param>
    /// <param name="payload">Payload associated with proxy.</param>
    /// <returns>Identifier of created proxy.</returns>
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

    /// <summary>
    /// Removes proxy identified by <paramref name="id"/> from grid.
    /// </summary>
    /// <param name="id">Identifier of proxy to remove.</param>
    /// <exception cref="InvalidOperationException"><paramref name="id"/> is invalid.</exception>
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

    /// <summary>
    /// Gets current bounds and payload of proxy.
    /// </summary>
    /// <param name="id">Identifier of proxy.</param>
    /// <returns>Proxy data for specified proxy.</returns>
    /// <exception cref="InvalidOperationException"><paramref name="id"/> is invalid.</exception>
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

    /// <summary>
    /// Updates proxy bounds and reinserts proxy into overlapped cells.
    /// </summary>
    /// <param name="id">Identifier of proxy to move.</param>
    /// <param name="newBounds">New axis-aligned bounding box of proxy.</param>
    /// <exception cref="InvalidOperationException"><paramref name="id"/> is invalid.</exception>
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

    /// <summary>
    /// Queries proxies containing specified point and reports matches as proxy identifiers.
    /// </summary>
    /// <typeparam name="TQueryHandler">Type of query handler.</typeparam>
    /// <param name="point">Point to test.</param>
    /// <param name="handler">Handler invoked for each match.</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void QueryPointAsId<TQueryHandler>(in Vector2 point, ref TQueryHandler handler) where TQueryHandler : struct, IProxyIdQueryHandler
    {
        QueryPoint(point, ref handler, static (ref TQueryHandler handler, in Node node, in Proxy<TPayload> proxy) =>
        {
            var proxyId = new SpatialGridProxyId(node.ProxyIndex, proxy.Version);
            return handler.Handle(proxyId);
        });
    }

    /// <summary>
    /// Queries proxies containing specified point and reports matches as payloads.
    /// </summary>
    /// <typeparam name="TQueryHandler">Type of query handler.</typeparam>
    /// <param name="point">Point to test.</param>
    /// <param name="handler">Handler invoked for each match.</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void QueryPointAsPayload<TQueryHandler>(in Vector2 point, ref TQueryHandler handler)
        where TQueryHandler : struct, IProxyPayloadQueryHandler<TPayload>
    {
        QueryPoint(point, ref handler, static (ref TQueryHandler handler, in Node _, in Proxy<TPayload> proxy) => handler.Handle(proxy.Payload));
    }

    /// <summary>
    /// Queries proxies overlapping specified bounds and reports matches as proxy identifiers.
    /// </summary>
    /// <typeparam name="TQueryHandler">Type of query handler.</typeparam>
    /// <param name="bounds">Bounds to test.</param>
    /// <param name="handler">Handler invoked for each match.</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void QueryBoundsAsId<TQueryHandler>(in AABB2D bounds, ref TQueryHandler handler) where TQueryHandler : struct, IProxyIdQueryHandler
    {
        QueryBounds(bounds, ref handler, static (ref TQueryHandler handler, in Node node, in Proxy<TPayload> proxy) =>
        {
            var proxyId = new SpatialGridProxyId(node.ProxyIndex, proxy.Version);
            return handler.Handle(proxyId);
        });
    }

    /// <summary>
    /// Queries proxies overlapping specified bounds and reports matches as payloads.
    /// </summary>
    /// <typeparam name="TQueryHandler">Type of query handler.</typeparam>
    /// <param name="bounds">Bounds to test.</param>
    /// <param name="handler">Handler invoked for each match.</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void QueryBoundsAsPayload<TQueryHandler>(in AABB2D bounds, ref TQueryHandler handler)
        where TQueryHandler : struct, IProxyPayloadQueryHandler<TPayload>
    {
        QueryBounds(bounds, ref handler, static (ref TQueryHandler handler, in Node _, in Proxy<TPayload> proxy) => handler.Handle(proxy.Payload));
    }

    /// <summary>
    /// Queries overlapping proxy pairs and reports matches as proxy identifiers.
    /// </summary>
    /// <typeparam name="TQueryHandler">Type of query handler.</typeparam>
    /// <param name="handler">Handler invoked for each matching pair.</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void QueryOverlappingPairsAsId<TQueryHandler>(ref TQueryHandler handler) where TQueryHandler : struct, IProxyIdPairQueryHandler
    {
        QueryOverlappingPairs(ref handler,
            static (ref TQueryHandler handler, in Node node1, in Node node2, in Proxy<TPayload> proxy1, in Proxy<TPayload> proxy2) =>
            {
                var proxyId1 = new SpatialGridProxyId(node1.ProxyIndex, proxy1.Version);
                var proxyId2 = new SpatialGridProxyId(node2.ProxyIndex, proxy2.Version);

                return handler.Handle(proxyId1, proxyId2);
            }
        );
    }

    /// <summary>
    /// Queries overlapping proxy pairs and reports matches as payload pairs.
    /// </summary>
    /// <typeparam name="TQueryHandler">Type of query handler.</typeparam>
    /// <param name="handler">Handler invoked for each matching pair.</param>
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public void QueryOverlappingPairsAsPayload<TQueryHandler>(ref TQueryHandler handler) where TQueryHandler : struct, IProxyPayloadPairQueryHandler<TPayload>
    {
        QueryOverlappingPairs(ref handler,
            static (ref TQueryHandler handler, in Node _, in Node _, in Proxy<TPayload> proxy1, in Proxy<TPayload> proxy2) =>
                handler.Handle(proxy1.Payload, proxy2.Payload)
        );
    }

    private delegate bool HandleProxyFunc<TQueryHandler>(ref TQueryHandler handler, in Node node, in Proxy<TPayload> proxy) where TQueryHandler : struct;

    private delegate bool HandlePairFunc<TQueryHandler>(ref TQueryHandler handler, in Node node1, in Node node2, in Proxy<TPayload> proxy1,
        in Proxy<TPayload> proxy2) where TQueryHandler : struct;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void QueryPoint<TQueryHandler>(in Vector2 point, ref TQueryHandler handler, HandleProxyFunc<TQueryHandler> handleProxyFunc)
        where TQueryHandler : struct
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
                shouldContinue = handleProxyFunc(ref handler, node, proxy);
            }

            nodeIndex = node.NextCellNodeIndex;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void QueryBounds<TQueryHandler>(in AABB2D bounds, ref TQueryHandler handler, HandleProxyFunc<TQueryHandler> handleProxyFunc)
        where TQueryHandler : struct
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
                        shouldContinue = handleProxyFunc(ref handler, node, proxy);
                    }
                }

                nodeIndex = node.NextCellNodeIndex;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private void QueryOverlappingPairs<TQueryHandler>(ref TQueryHandler handler, HandlePairFunc<TQueryHandler> handlePairFunc)
        where TQueryHandler : struct
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

                    if (proxy1.Bounds.Overlaps(proxy2.Bounds))
                    {
                        var intersection = proxy1.Bounds.Intersect(proxy2.Bounds);
                        Debug.Assert(intersection.IsValid);

                        var canonicalCell = FindCell(intersection.Min);

                        // Pair must be handled only in single canonical cell to avoid duplicates.
                        if (cell.Key == canonicalCell.Key)
                        {
                            shouldContinue = handlePairFunc(ref handler, node1, node2, proxy1, proxy2);
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