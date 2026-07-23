using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Geisha.Engine.Core.Math;
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
        var proxyIds = InitScratchBuffer();
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
            ClearScratchBuffer(proxyIds);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void DetectCollisions_Kinematic_Vs_Kinematic(ref PhysicsSceneData scene)
    {
        var kinematicBodiesSpan = scene.GetKinematicBodiesSpan();

        for (var i = 0; i < kinematicBodiesSpan.Length; i++)
        {
            ref var kinematicBody1 = ref kinematicBodiesSpan[i];

            for (var j = i + 1; j < kinematicBodiesSpan.Length; j++)
            {
                ref var kinematicBody2 = ref kinematicBodiesSpan[j];

                if (kinematicBody1.EnableCollisionDetection is false || kinematicBody2.EnableCollisionDetection is false)
                {
                    continue;
                }

                if ((kinematicBody1.CollisionLayer & kinematicBody2.CollisionMask) == 0 || (kinematicBody1.CollisionMask & kinematicBody2.CollisionLayer) == 0)
                {
                    continue;
                }

                if (!TestAABB(ref kinematicBody1, ref kinematicBody2))
                {
                    continue;
                }

                if (kinematicBody1.IsSensor || kinematicBody2.IsSensor)
                {
                    if (TestOverlap(ref kinematicBody1, ref kinematicBody2))
                    {
                        scene.SensorOverlapCache.AddPair(kinematicBody1.Id, kinematicBody2.Id);
                    }
                }
                else
                {
                    var (overlap, mtv) = TestOverlapWithMtv(ref kinematicBody1, ref kinematicBody2);

                    if (overlap)
                    {
                        ContactManager.CreateContact(ref scene, ref kinematicBody1, ref kinematicBody2, mtv);
                    }
                }
            }
        }
    }

    // This method is not part of TestOverlap because doing so breaks inlining and optimization of the method.
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    // ReSharper disable once InconsistentNaming
    private static bool TestAABB(ref RigidBodyData body1, ref RigidBodyData body2)
    {
        return body1.AABB.Overlaps(body2.AABB);
    }

    // TODO: Once broad phase is implemented in scope of https://github.com/dawidkomorowski/geisha/issues/608 the collider type switch logic could be investigated
    //       for deduplication - it will no longer be so hot path. Maybe there is some way to group all switch/case logic in single place for these type of operations.
    //       It could also benefit ContactManager as it also tests for combinations of colliding pairs.
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static bool TestOverlap(ref RigidBodyData body1, ref RigidBodyData body2)
    {
        var overlap = false;

        if (body1.ColliderType is ColliderType.Circle && body2.ColliderType is ColliderType.Circle)
        {
            overlap = body1.TransformedCircleCollider.Overlaps(body2.TransformedCircleCollider);
        }
        else if (body1.ColliderType is ColliderType.Circle && body2.ColliderType is ColliderType.Rectangle)
        {
            overlap = body1.TransformedCircleCollider.Overlaps(body2.TransformedRectangleCollider);
        }
        else if (body1.ColliderType is ColliderType.Circle && body2.ColliderType is ColliderType.Tile)
        {
            overlap = body1.TransformedCircleCollider.Overlaps(body2.TransformedRectangleCollider);
        }
        else if (body1.ColliderType is ColliderType.Rectangle && body2.ColliderType is ColliderType.Circle)
        {
            overlap = body1.TransformedRectangleCollider.Overlaps(body2.TransformedCircleCollider);
        }
        else if (body1.ColliderType is ColliderType.Rectangle && body2.ColliderType is ColliderType.Rectangle)
        {
            overlap = body1.TransformedRectangleCollider.Overlaps(body2.TransformedRectangleCollider);
        }
        else if (body1.ColliderType is ColliderType.Rectangle && body2.ColliderType is ColliderType.Tile)
        {
            overlap = body1.TransformedRectangleCollider.Overlaps(body2.TransformedRectangleCollider);
        }

        return overlap;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static (bool overlap, MinimumTranslationVector mtv) TestOverlapWithMtv(ref RigidBodyData body1, ref RigidBodyData body2)
    {
        var overlap = false;
        var mtv = new MinimumTranslationVector();

        if (body1.ColliderType is ColliderType.Circle && body2.ColliderType is ColliderType.Circle)
        {
            overlap = body1.TransformedCircleCollider.Overlaps(body2.TransformedCircleCollider, out mtv);
        }
        else if (body1.ColliderType is ColliderType.Circle && body2.ColliderType is ColliderType.Rectangle)
        {
            overlap = body1.TransformedCircleCollider.Overlaps(body2.TransformedRectangleCollider, out mtv);
        }
        else if (body1.ColliderType is ColliderType.Circle && body2.ColliderType is ColliderType.Tile)
        {
            overlap = body1.TransformedCircleCollider.Overlaps(body2.TransformedRectangleCollider, out mtv);
        }
        else if (body1.ColliderType is ColliderType.Rectangle && body2.ColliderType is ColliderType.Circle)
        {
            overlap = body1.TransformedRectangleCollider.Overlaps(body2.TransformedCircleCollider, out mtv);
        }
        else if (body1.ColliderType is ColliderType.Rectangle && body2.ColliderType is ColliderType.Rectangle)
        {
            overlap = body1.TransformedRectangleCollider.Overlaps(body2.TransformedRectangleCollider, out mtv);
        }
        else if (body1.ColliderType is ColliderType.Rectangle && body2.ColliderType is ColliderType.Tile)
        {
            overlap = body1.TransformedRectangleCollider.Overlaps(body2.TransformedRectangleCollider, out mtv);
        }

        return (overlap, mtv);
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