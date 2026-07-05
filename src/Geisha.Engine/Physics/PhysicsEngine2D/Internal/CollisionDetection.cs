using System.Runtime.CompilerServices;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

// Watch out with refactoring this class! It is performance critical and should be kept as fast as possible.
// Trivial refactorings like combining methods or extracting methods can have a significant impact on performance.
internal static class CollisionDetection
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void DetectCollisions(ref PhysicsSceneData scene)
    {
        scene.SensorOverlapCache.RemoveStale();
        scene.SensorOverlapCache.MarkStale();

        ContactManager.ClearContacts(ref scene);

        DetectCollisions_Kinematic_Vs_Kinematic(ref scene);
        DetectCollisions_Kinematic_Vs_Static(ref scene);
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

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void DetectCollisions_Kinematic_Vs_Static(ref PhysicsSceneData scene)
    {
        var staticBodiesSpan = scene.GetStaticBodiesSpan();
        var kinematicBodiesSpan = scene.GetKinematicBodiesSpan();

        foreach (ref var kinematicBody in kinematicBodiesSpan)
        {
            foreach (ref var staticBody in staticBodiesSpan)
            {
                if (kinematicBody.EnableCollisionDetection is false || staticBody.EnableCollisionDetection is false)
                {
                    continue;
                }

                if ((kinematicBody.CollisionLayer & staticBody.CollisionMask) == 0 || (kinematicBody.CollisionMask & staticBody.CollisionLayer) == 0)
                {
                    continue;
                }

                if (!TestAABB(ref kinematicBody, ref staticBody))
                {
                    continue;
                }

                if (kinematicBody.IsSensor || staticBody.IsSensor)
                {
                    if (TestOverlap(ref kinematicBody, ref staticBody))
                    {
                        scene.SensorOverlapCache.AddPair(kinematicBody.Id, staticBody.Id);
                    }
                }
                else
                {
                    var (overlap, mtv) = TestOverlapWithMtv(ref kinematicBody, ref staticBody);

                    if (overlap)
                    {
                        ContactManager.CreateContact(ref scene, ref kinematicBody, ref staticBody, mtv);
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
}