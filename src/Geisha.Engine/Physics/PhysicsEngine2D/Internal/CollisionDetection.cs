using System;
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
        //sensorOverlapCache.RemoveStale();
        //sensorOverlapCache.MarkStale();

        //foreach (var staticBody in staticBodies)
        //{
        //    staticBody.Contacts.Clear();
        //}

        //foreach (var kinematicBody in kinematicBodies)
        //{
        //    kinematicBody.Contacts.Clear();
        //}

        DetectCollisions_Kinematic_Vs_Kinematic(ref scene);
        DetectCollisions_Kinematic_Vs_Static(ref scene);
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void DetectCollisions_Kinematic_Vs_Kinematic(ref PhysicsSceneData scene)
    {
        for (var i = 0; i < scene.KinematicBodyIndices.Count; i++)
        {
            ref var kinematicBody1 = ref scene.BodiesSpan[i];

            for (var j = i + 1; j < scene.KinematicBodyIndices.Count; j++)
            {
                ref var kinematicBody2 = ref scene.BodiesSpan[j];

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
                        //sensorOverlapCache.AddPair(kinematicBody1, kinematicBody2);
                    }
                }
                else
                {
                    var (overlap, mtv) = TestOverlapWithMtv(ref kinematicBody1, ref kinematicBody2);

                    if (overlap)
                    {
                        //var contact = ContactGenerator.GenerateContact(kinematicBody1, kinematicBody2, mtv);
                        //kinematicBody1.Contacts.Add(contact);
                        //kinematicBody2.Contacts.Add(contact);
                    }
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static void DetectCollisions_Kinematic_Vs_Static(ref PhysicsSceneData scene)
    {
        foreach (var kinematicBodyIndex in scene.KinematicBodyIndices)
        {
            ref var kinematicBody = ref scene.BodiesSpan[kinematicBodyIndex];

            foreach (var staticBodyIndex in scene.StaticBodyIndices)
            {
                ref var staticBody = ref scene.BodiesSpan[staticBodyIndex];

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
                        //sensorOverlapCache.AddPair(kinematicBody, staticBody);
                    }
                }
                else
                {
                    var (overlap, mtv) = TestOverlapWithMtv(ref kinematicBody, ref staticBody);

                    if (overlap)
                    {
                        //var contact = ContactGenerator.GenerateContact(kinematicBody, staticBody, mtv);
                        //kinematicBody.Contacts.Add(contact);
                        //staticBody.Contacts.Add(contact);
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
        return body1.BoundingRectangle.Overlaps(body2.BoundingRectangle);
    }

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