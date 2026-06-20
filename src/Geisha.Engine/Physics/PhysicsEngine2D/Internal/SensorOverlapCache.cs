using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal enum CacheStatus
{
    New,
    Updated,
    Stale
}

internal struct SensorOverlap
{
    public SensorOverlap(RigidBodyId body1Id, RigidBodyId body2Id)
    {
        Body1Id = body1Id;
        Body2Id = body2Id;
        CacheStatus = CacheStatus.New;
    }

    public RigidBodyId Body1Id { get; }
    public RigidBodyId Body2Id { get; }

    public CacheStatus CacheStatus { get; set; }
}

internal sealed class SensorOverlapCache
{
    private readonly List<SensorOverlap> _overlaps;
    private readonly Dictionary<CacheKey, int> _index;

    public SensorOverlapCache(int capacity)
    {
        _overlaps = new List<SensorOverlap>(capacity);
        _index = new Dictionary<CacheKey, int>(capacity);
    }

    public void AddPair(RigidBodyId body1Id, RigidBodyId body2Id)
    {
        Debug.Assert(body1Id != body2Id, "Overlap pair is invalid: body1Id == body2Id");

        var cacheKey = new CacheKey(body1Id, body2Id);

        if (_index.TryGetValue(cacheKey, out var i))
        {
            GetOverlapsAsSpan()[i].CacheStatus = CacheStatus.Updated;
        }
        else
        {
            _overlaps.Add(new SensorOverlap(body1Id, body2Id));
            _index.Add(cacheKey, _overlaps.Count - 1);
        }
    }

    public void MarkStale()
    {
        foreach (ref var overlap in GetOverlapsAsSpan())
        {
            overlap.CacheStatus = CacheStatus.Stale;
        }
    }

    public void RemoveStale()
    {
        for (var i = 0; i < _overlaps.Count; i++)
        {
            if (_overlaps[i].CacheStatus is not CacheStatus.Stale)
            {
                continue;
            }

            var overlap = _overlaps[i];
            var cacheKey = new CacheKey(overlap.Body1Id, overlap.Body2Id);
            _index.Remove(cacheKey);

            if (i == _overlaps.Count - 1)
            {
                // If the last element is being removed, just remove it.
                _overlaps.RemoveAt(i);
            }
            else
            {
                // Otherwise swap-remove with last element.
                _overlaps[i] = _overlaps[^1];
                _overlaps.RemoveAt(_overlaps.Count - 1);

                // Update index pointer.
                overlap = _overlaps[i];
                cacheKey = new CacheKey(overlap.Body1Id, overlap.Body2Id);
                _index[cacheKey] = i;

                // Decrement i to inspect swapped element in next iteration.
                i--;
            }
        }
    }

    private Span<SensorOverlap> GetOverlapsAsSpan() => CollectionsMarshal.AsSpan(_overlaps);
    public ReadOnlySpan<SensorOverlap> GetOverlaps() => GetOverlapsAsSpan();

    private readonly record struct CacheKey
    {
        public readonly int Body1Id;
        public readonly int Body2Id;

        public CacheKey(RigidBodyId body1Id, RigidBodyId body2Id)
        {
            if (body1Id.Index < body2Id.Index)
            {
                Body1Id = body1Id.Index;
                Body2Id = body2Id.Index;
            }
            else
            {
                Body1Id = body2Id.Index;
                Body2Id = body1Id.Index;
            }
        }
    }
}