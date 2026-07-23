using System;
using System.Collections.Generic;
using System.Diagnostics;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.Spatial;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

// TODO: Review and possibly update related tests to cover new implementation.
//       Now queries rely on spatial grid so bugs in updates of spatial grid should be captured in query tests?
internal static class SceneQuery
{
    public static void QueryPoint<TQueryHandler>(in PhysicsSceneData scene, in Vector2 point, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler
    {
        // 1. Init
        var proxyIds = InitScratchBuffer();
        var proxyHandler = new ProxyQueryHandler(proxyIds);

        try
        {
            // 2. Gather
            scene.StaticGrid.QueryPoint(point, ref proxyHandler);
            scene.DynamicGrid.QueryPoint(point, ref proxyHandler);

            // 3. Process
            foreach (var proxyId in proxyIds)
            {
                var bodyId = scene.StaticGrid.IsValidProxy(proxyId)
                    ? scene.StaticGrid.GetProxyData(proxyId).Payload
                    : scene.DynamicGrid.GetProxyData(proxyId).Payload;

                ref var body = ref scene.GetBodyData(bodyId);
                if (body.ContainsPoint(point))
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

    public static void QueryBounds<TQueryHandler>(in PhysicsSceneData scene, in AABB2D aabb, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler
    {
        QueryByBounds(scene, aabb, ref handler, aabb, static (in RigidBodyData body, in AABB2D queryArg) => body.AABB.Overlaps(queryArg));
    }

    public static void QueryOverlap<TQueryHandler>(in PhysicsSceneData scene, in AABB2D aabb, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler
    {
        QueryByBounds(scene, aabb, ref handler, aabb, static (in RigidBodyData body, in AABB2D queryArg) => body.Overlaps(queryArg));
    }

    public static void QueryOverlap<TQueryHandler>(in PhysicsSceneData scene, in Circle circle, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler
    {
        QueryByBounds(scene, circle.ComputeAABB(), ref handler, circle, static (in RigidBodyData body, in Circle queryArg) => body.Overlaps(queryArg));
    }

    public static void QueryOverlap<TQueryHandler>(in PhysicsSceneData scene, in Rectangle rectangle, ref TQueryHandler handler)
        where TQueryHandler : struct, IRigidBodyIdQueryHandler
    {
        QueryByBounds(scene, rectangle.ComputeAABB(), ref handler, rectangle, static (in RigidBodyData body, in Rectangle queryArg) => body.Overlaps(queryArg));
    }

    private delegate bool QueryFunc<TQueryArg>(in RigidBodyData body, in TQueryArg queryArg);

    private static void QueryByBounds<TQueryHandler, TQueryArg>(in PhysicsSceneData scene, in AABB2D bounds, ref TQueryHandler handler,
        in TQueryArg queryArg, QueryFunc<TQueryArg> queryFunc
    )
        where TQueryHandler : struct, IRigidBodyIdQueryHandler
        where TQueryArg : struct
    {
        // 1. Init
        var proxyIds = InitScratchBuffer();
        var proxyHandler = new ProxyQueryHandler(proxyIds);

        try
        {
            // 2. Gather
            scene.StaticGrid.QueryBounds(bounds, ref proxyHandler);
            scene.DynamicGrid.QueryBounds(bounds, ref proxyHandler);

            // 3. Process
            foreach (var proxyId in proxyIds)
            {
                var bodyId = scene.StaticGrid.IsValidProxy(proxyId)
                    ? scene.StaticGrid.GetProxyData(proxyId).Payload
                    : scene.DynamicGrid.GetProxyData(proxyId).Payload;

                ref var body = ref scene.GetBodyData(bodyId);
                if (queryFunc(body, in queryArg))
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