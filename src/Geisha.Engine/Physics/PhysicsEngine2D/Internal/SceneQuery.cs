using System;
using System.Collections.Generic;
using System.Diagnostics;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Spatial;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

// TODO: Implement remaining queries using spatial grid.
// TODO: Review and possibly update related tests to cover new implementation.
//       Now queries rely on spatial grid so bugs in updates of spatial grid should be captured in query tests.
internal static class SceneQuery
{
    public static void QueryPoint<TQueryHandler>(in PhysicsSceneData scene, in Vector2 point, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler
    {
        foreach (ref var body in scene.GetBodiesSpan())
        {
            if (body.ContainsPoint(point))
            {
                if (!handler.Handle(body.Id))
                {
                    return;
                }
            }
        }
    }

    public static void QueryBounds<TQueryHandler>(in PhysicsSceneData scene, in AABB2D aabb, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler
    {
        // 1. Init
        var proxyIds = InitScratchBuffer();
        var proxyHandler = new ProxyQueryHandler(proxyIds);

        try
        {
            // 2. Gather
            scene.StaticGrid.QueryBounds(aabb, ref proxyHandler);
            scene.DynamicGrid.QueryBounds(aabb, ref proxyHandler);

            // 3. Process
            foreach (var proxyId in proxyIds)
            {
                var bodyId = scene.StaticGrid.IsValidProxy(proxyId)
                    ? scene.StaticGrid.GetProxyData(proxyId).Payload
                    : scene.DynamicGrid.GetProxyData(proxyId).Payload;

                ref var body = ref scene.GetBodyData(bodyId);
                if (body.AABB.Overlaps(aabb))
                {
                    if (!handler.Handle(body.Id))
                    {
                        return;
                    }
                }
            }
        }
        finally
        {
            // 4. Cleanup (guaranteed execution even if an exception is thrown or an early return happens)
            ClearScratchBuffer(proxyIds);
        }
    }

    public static void QueryOverlap<TQueryHandler>(in PhysicsSceneData scene, in AABB2D aabb, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler
    {
        // 1. Init
        var proxyIds = InitScratchBuffer();
        var proxyHandler = new ProxyQueryHandler(proxyIds);

        try
        {
            // 2. Gather
            scene.StaticGrid.QueryBounds(aabb, ref proxyHandler);
            scene.DynamicGrid.QueryBounds(aabb, ref proxyHandler);

            // 3. Process
            foreach (var proxyId in proxyIds)
            {
                var bodyId = scene.StaticGrid.IsValidProxy(proxyId)
                    ? scene.StaticGrid.GetProxyData(proxyId).Payload
                    : scene.DynamicGrid.GetProxyData(proxyId).Payload;

                ref var body = ref scene.GetBodyData(bodyId);
                if (body.Overlaps(aabb))
                {
                    if (!handler.Handle(body.Id))
                    {
                        return;
                    }
                }
            }
        }
        finally
        {
            // 4. Cleanup (guaranteed execution even if an exception is thrown or an early return happens)
            ClearScratchBuffer(proxyIds);
        }
    }

    public static void QueryOverlap<TQueryHandler>(in PhysicsSceneData scene, in Circle circle, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler
    {
        foreach (ref var body in scene.GetBodiesSpan())
        {
            if (body.Overlaps(circle))
            {
                if (!handler.Handle(body.Id))
                {
                    return;
                }
            }
        }
    }

    public static void QueryOverlap<TQueryHandler>(in PhysicsSceneData scene, in Rectangle rectangle, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler
    {
        foreach (ref var body in scene.GetBodiesSpan())
        {
            if (body.Overlaps(rectangle))
            {
                if (!handler.Handle(body.Id))
                {
                    return;
                }
            }
        }
    }

    // TODO: To implement ProxyQueryHandler properly it requires ref fields and ref struct interfaces features of .NET 9 (C# 13).
    //       Once upgraded to .NET 9, refactor it to use ref fields and ref struct interfaces and implement single pass query logic.
    //       -----------------------------------------------------------------------------------------------------------------------------
    //       As a workaround, a static scratch buffer is used to do double-pass gather-then-process query logic.
    //       [ThreadStatic] ensures every thread gets its own isolated buffer. This prevents thread collisions if queries run in parallel.
    //       However, this implementation does not support reentrancy.
    [ThreadStatic] private static List<SpatialGridProxyId>? _scratchBuffer;

    private static List<SpatialGridProxyId> InitScratchBuffer()
    {
        _scratchBuffer ??= new List<SpatialGridProxyId>(2048);

        Debug.Assert(_scratchBuffer.Count == 0, "Reentrancy is not yet supported.");

        return _scratchBuffer;
    }

    private static void ClearScratchBuffer(List<SpatialGridProxyId> buffer)
    {
        Debug.Assert(_scratchBuffer == buffer, "Invalid buffer.");
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
}