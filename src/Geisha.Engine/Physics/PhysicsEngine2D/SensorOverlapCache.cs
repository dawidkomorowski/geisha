using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Geisha.Engine.Core;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal enum CacheStatus
{
    New,
    Updated,
    Stale
}

internal struct SensorOverlap
{
    public SensorOverlap(RigidBody2D body1, RigidBody2D body2)
    {
        Body1 = body1;
        Body2 = body2;
        CacheStatus = CacheStatus.New;
    }

    public RigidBody2D Body1 { get; }
    public RigidBody2D Body2 { get; }

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

    public void AddPair(RigidBody2D body1, RigidBody2D body2)
    {
        Debug.Assert(body1 != body2, "Overlap pair is invalid: body1 == body2");

        var cacheKey = new CacheKey(body1, body2);

        if (_index.TryGetValue(cacheKey, out var i))
        {
            GetOverlapsAsSpan()[i].CacheStatus = CacheStatus.Updated;
        }
        else
        {
            _overlaps.Add(new SensorOverlap(body1, body2));
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
            var cacheKey = new CacheKey(overlap.Body1, overlap.Body2);
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
                cacheKey = new CacheKey(overlap.Body1, overlap.Body2);
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
        public CacheKey(RigidBody2D body1, RigidBody2D body2)
        {
            if (body1.Id.CompareTo(body2.Id) < 0)
            {
                Body1Id = body1.Id;
                Body2Id = body2.Id;
            }
            else
            {
                Body1Id = body2.Id;
                Body2Id = body1.Id;
            }
        }

        public RuntimeId Body1Id { get; }
        public RuntimeId Body2Id { get; }
    }
}