using System.Runtime.CompilerServices;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

internal static class NarrowPhase
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void DetectCollision(ref PhysicsSceneData scene, ref RigidBodyData body1, ref RigidBodyData body2)
    {
        if (body1.EnableCollisionDetection is false || body2.EnableCollisionDetection is false)
        {
            return;
        }

        if ((body1.CollisionLayer & body2.CollisionMask) == 0 || (body1.CollisionMask & body2.CollisionLayer) == 0)
        {
            return;
        }

        if (!body1.AABB.Overlaps(body2.AABB))
        {
            return;
        }

        if (body1.IsSensor || body2.IsSensor)
        {
            if (TestOverlap(ref body1, ref body2))
            {
                scene.SensorOverlapCache.AddPair(body1.Id, body2.Id);
            }
        }
        else
        {
            var (overlap, mtv) = TestOverlapWithMtv(ref body1, ref body2);

            if (overlap)
            {
                ContactManager.CreateContact(ref scene, ref body1, ref body2, mtv);
            }
        }
    }

    // TODO: The collider type dispatch is duplicated across TestOverlap, TestOverlapWithMtv and ContactManager.ComputeManifold,
    //       each enumerating the same combinations of collider pairs. There is no obvious way to deduplicate it without deeper
    //       design work, likely introducing some auxiliary generic shape representation to dispatch on, so it is left as is for
    //       now. Worth revisiting as the number of supported geometry shapes grows, since each new shape adds a row and a column
    //       to every one of these dispatches. Keep in mind that this is a hot path and the current shape is deliberate.
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