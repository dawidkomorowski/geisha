using Geisha.Engine.Core.Math;
using System;
using System.Runtime.CompilerServices;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

// Watch out with refactoring this class! It is performance critical and should be kept as fast as possible.
// Trivial refactorings like combining methods or extracting methods can have a significant impact on performance.
internal static class CollisionDetection
{
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static void DetectCollisions(ReadOnlySpan<RigidBody2D> staticBodies, ReadOnlySpan<RigidBody2D> kinematicBodies,
        SensorOverlapCache sensorOverlapCache)
    {
        sensorOverlapCache.RemoveStale();
        sensorOverlapCache.MarkStale();

        foreach (var staticBody in staticBodies)
        {
            staticBody.Contacts.Clear();
        }

        foreach (var kinematicBody in kinematicBodies)
        {
            kinematicBody.Contacts.Clear();
        }

        DetectCollisions_Kinematic_Vs_Kinematic(kinematicBodies, sensorOverlapCache);
        DetectCollisions_Kinematic_Vs_Static(kinematicBodies, staticBodies, sensorOverlapCache);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void DetectCollisions_Kinematic_Vs_Kinematic(ReadOnlySpan<RigidBody2D> kinematicBodies, SensorOverlapCache sensorOverlapCache)
    {
        for (var i = 0; i < kinematicBodies.Length; i++)
        {
            var kinematicBody1 = kinematicBodies[i];

            for (var j = i + 1; j < kinematicBodies.Length; j++)
            {
                var kinematicBody2 = kinematicBodies[j];

                if (kinematicBody1.EnableCollisionDetection is false || kinematicBody2.EnableCollisionDetection is false)
                {
                    continue;
                }

                if ((kinematicBody1.CollisionLayer & kinematicBody2.CollisionMask) == 0 || (kinematicBody1.CollisionMask & kinematicBody2.CollisionLayer) == 0)
                {
                    continue;
                }

                if (!TestAABB(kinematicBody1, kinematicBody2))
                {
                    continue;
                }

                if (kinematicBody1.IsSensor || kinematicBody2.IsSensor)
                {
                    if (TestOverlap(kinematicBody1, kinematicBody2))
                    {
                        sensorOverlapCache.AddPair(kinematicBody1, kinematicBody2);
                    }
                }
                else
                {
                    var (overlap, mtv) = TestOverlapWithMtv(kinematicBody1, kinematicBody2);

                    if (overlap)
                    {
                        var contact = ContactGenerator.GenerateContact(kinematicBody1, kinematicBody2, mtv);
                        kinematicBody1.Contacts.Add(contact);
                        kinematicBody2.Contacts.Add(contact);
                    }
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void DetectCollisions_Kinematic_Vs_Static(ReadOnlySpan<RigidBody2D> kinematicBodies, ReadOnlySpan<RigidBody2D> staticBodies,
        SensorOverlapCache sensorOverlapCache)
    {
        foreach (var kinematicBody in kinematicBodies)
        {
            foreach (var staticBody in staticBodies)
            {
                if (kinematicBody.EnableCollisionDetection is false || staticBody.EnableCollisionDetection is false)
                {
                    continue;
                }

                if ((kinematicBody.CollisionLayer & staticBody.CollisionMask) == 0 || (kinematicBody.CollisionMask & staticBody.CollisionLayer) == 0)
                {
                    continue;
                }

                if (!TestAABB(kinematicBody, staticBody))
                {
                    continue;
                }

                if (kinematicBody.IsSensor || staticBody.IsSensor)
                {
                    if (TestOverlap(kinematicBody, staticBody))
                    {
                        sensorOverlapCache.AddPair(kinematicBody, staticBody);
                    }
                }
                else
                {
                    var (overlap, mtv) = TestOverlapWithMtv(kinematicBody, staticBody);

                    if (overlap)
                    {
                        var contact = ContactGenerator.GenerateContact(kinematicBody, staticBody, mtv);
                        kinematicBody.Contacts.Add(contact);
                        staticBody.Contacts.Add(contact);
                    }
                }
            }
        }
    }

    // This method is not part of TestOverlap because doing so breaks inlining and optimization of the method.
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    // ReSharper disable once InconsistentNaming
    private static bool TestAABB(RigidBody2D body1, RigidBody2D body2)
    {
        return body1.BoundingRectangle.Overlaps(body2.BoundingRectangle);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    private static bool TestOverlap(RigidBody2D body1, RigidBody2D body2)
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
    private static (bool overlap, MinimumTranslationVector mtv) TestOverlapWithMtv(RigidBody2D body1, RigidBody2D body2)
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