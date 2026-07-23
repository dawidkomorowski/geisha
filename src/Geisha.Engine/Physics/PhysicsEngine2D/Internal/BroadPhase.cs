using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Geisha.Engine.Core.Spatial;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

// TODO: Review and possibly update related tests to cover new implementation.
//       Now collision detection rely on spatial grid so bugs in updates of spatial grid should be captured in collision detection tests?
internal static class BroadPhase
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void DetectCollisions(ref PhysicsSceneData scene)
    {
        scene.SensorOverlapCache.RemoveStale();
        scene.SensorOverlapCache.MarkStale();

        ContactManager.ClearContacts(ref scene);

        DetectCollisions_Kinematic_Vs_Static(ref scene);
        DetectCollisions_Kinematic_Vs_Kinematic(ref scene);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void DetectCollisions_Kinematic_Vs_Static(ref PhysicsSceneData scene)
    {
        // 1. Init
        var proxyIds = InitProxyScratchBuffer();
        var proxyHandler = new ProxyQueryHandler(proxyIds);

        try
        {
            var kinematicBodiesSpan = scene.GetKinematicBodiesSpan();
            foreach (ref var kinematicBody in kinematicBodiesSpan)
            {
                // 2. Gather
                proxyIds.Clear();
                scene.StaticGrid.QueryBounds(kinematicBody.AABB, ref proxyHandler);

                // 3. Process
                foreach (var proxyId in proxyIds)
                {
                    var bodyId = scene.StaticGrid.GetProxyData(proxyId).Payload;
                    ref var staticBody = ref scene.GetBodyData(bodyId);

                    NarrowPhase.DetectCollision(ref scene, ref kinematicBody, ref staticBody);
                }
            }
        }
        finally
        {
            // 4. Cleanup (guaranteed execution even if an exception is thrown or an early return happens)
            ClearProxyScratchBuffer(proxyIds);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void DetectCollisions_Kinematic_Vs_Kinematic(ref PhysicsSceneData scene)
    {
        // 1. Init
        var pairs = InitPairScratchBuffer();
        var pairHandler = new PairsQueryHandler(pairs);

        try
        {
            // 2. Gather
            scene.DynamicGrid.QueryOverlappingPairs(ref pairHandler);

            // 3. Process
            foreach (var pair in pairs)
            {
                var bodyId1 = scene.DynamicGrid.GetProxyData(pair.ProxyId1).Payload;
                var bodyId2 = scene.DynamicGrid.GetProxyData(pair.ProxyId2).Payload;

                ref var kinematicBody1 = ref scene.GetBodyData(bodyId1);
                ref var kinematicBody2 = ref scene.GetBodyData(bodyId2);

                NarrowPhase.DetectCollision(ref scene, ref kinematicBody1, ref kinematicBody2);
            }
        }
        finally
        {
            // 4. Cleanup (guaranteed execution even if an exception is thrown or an early return happens)
            ClearPairScratchBuffer(pairs);
        }
    }

    // TODO: To implement ProxyQueryHandler properly it requires ref fields and ref struct interfaces features of .NET 9 (C# 13).
    //       Once upgraded to .NET 9, refactor it to use ref fields and ref struct interfaces and implement single pass query logic.
    //       -----------------------------------------------------------------------------------------------------------------------------
    //       As a workaround, a static scratch buffer is used to do double-pass gather-then-process query logic.
    //       [ThreadStatic] ensures every thread gets its own isolated buffer. This prevents thread collisions if queries run in parallel.
    //       However, this implementation does not support reentrancy.
    [ThreadStatic] private static List<SpatialGridProxyId>? _proxyScratchBuffer;

    private static List<SpatialGridProxyId> InitProxyScratchBuffer()
    {
        _proxyScratchBuffer ??= new List<SpatialGridProxyId>(2048);

        Debug.Assert(_proxyScratchBuffer.Count == 0, "Reentrancy is not yet supported.");

        return _proxyScratchBuffer;
    }

    private static void ClearProxyScratchBuffer(List<SpatialGridProxyId> buffer)
    {
        Debug.Assert(_proxyScratchBuffer == buffer, "Invalid buffer.");
        buffer.Clear();
    }

    private readonly struct ProxyQueryHandler : IProxyQueryHandler
    {
        private readonly List<SpatialGridProxyId> _proxies;

        public ProxyQueryHandler(List<SpatialGridProxyId> proxies)
        {
            _proxies = proxies;
        }

        public bool Handle(SpatialGridProxyId proxyId)
        {
            _proxies.Add(proxyId);
            return true;
        }
    }

    // TODO: To implement ProxyQueryHandler properly it requires ref fields and ref struct interfaces features of .NET 9 (C# 13).
    //       Once upgraded to .NET 9, refactor it to use ref fields and ref struct interfaces and implement single pass query logic.
    //       -----------------------------------------------------------------------------------------------------------------------------
    //       As a workaround, a static scratch buffer is used to do double-pass gather-then-process query logic.
    //       [ThreadStatic] ensures every thread gets its own isolated buffer. This prevents thread collisions if queries run in parallel.
    //       However, this implementation does not support reentrancy.
    [ThreadStatic] private static List<Pair>? _pairScratchBuffer;

    private static List<Pair> InitPairScratchBuffer()
    {
        _pairScratchBuffer ??= new List<Pair>(2048);

        Debug.Assert(_pairScratchBuffer.Count == 0, "Reentrancy is not yet supported.");

        return _pairScratchBuffer;
    }

    private static void ClearPairScratchBuffer(List<Pair> buffer)
    {
        Debug.Assert(_pairScratchBuffer == buffer, "Invalid buffer.");
        buffer.Clear();
    }

    private readonly record struct Pair(SpatialGridProxyId ProxyId1, SpatialGridProxyId ProxyId2);

    private readonly struct PairsQueryHandler : IPairsQueryHandler
    {
        private readonly List<Pair> _pairs;

        public PairsQueryHandler(List<Pair> pairs)
        {
            _pairs = pairs;
        }

        public bool Handle(SpatialGridProxyId proxyId1, SpatialGridProxyId proxyId2)
        {
            _pairs.Add(new Pair(proxyId1, proxyId2));
            return true;
        }
    }
}