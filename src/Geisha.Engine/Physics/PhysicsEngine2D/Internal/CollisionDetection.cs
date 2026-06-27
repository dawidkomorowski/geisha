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

        ClearContacts(ref scene);

        DetectCollisions_Kinematic_Vs_Kinematic(ref scene);
        DetectCollisions_Kinematic_Vs_Static(ref scene);
    }

    private static void ClearContacts(ref PhysicsSceneData scene)
    {
        scene.Contacts.Clear();

        foreach (ref var body in scene.GetBodiesSpan())
        {
            body.ContactCount = 0;
            body.FirstContactIndex = ContactData.Link.NullIndex;
            body.LastContactIndex = ContactData.Link.NullIndex;
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
                        CreateContact(ref scene, ref kinematicBody1, ref kinematicBody2, mtv);
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
                        CreateContact(ref scene, ref kinematicBody, ref staticBody, mtv);
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

    private static void CreateContact(ref PhysicsSceneData scene, ref RigidBodyData body1, ref RigidBodyData body2, in MinimumTranslationVector mtv)
    {
        var contactManifold = ContactManifoldAnalyzer.FindManifold(in body1, in body2, mtv);

        ContactData contact = default;
        contact.ContactManifold = contactManifold;
        contact.Link1.BodyId = body1.Id;
        contact.Link1.PrevIndex = body1.LastContactIndex;
        contact.Link1.NextIndex = ContactData.Link.NullIndex;
        contact.Link2.BodyId = body2.Id;
        contact.Link2.PrevIndex = body2.LastContactIndex;
        contact.Link2.NextIndex = ContactData.Link.NullIndex;

        var currentIndex = scene.Contacts.Count;
        scene.Contacts.Add(contact);
        var contactsSpan = scene.GetContactsSpan();

        if (body1.ContactCount == 0)
        {
            body1.FirstContactIndex = currentIndex;
        }
        else
        {
            ref var prevContact = ref contactsSpan[body1.LastContactIndex];
            ref var link = ref prevContact.Link1.BodyId == body1.Id ? ref prevContact.Link1 : ref prevContact.Link2;
            link.NextIndex = currentIndex;
        }

        body1.LastContactIndex = currentIndex;
        body1.ContactCount++;

        if (body2.ContactCount == 0)
        {
            body2.FirstContactIndex = currentIndex;
        }
        else
        {
            ref var prevContact = ref contactsSpan[body2.LastContactIndex];
            ref var link = ref prevContact.Link1.BodyId == body2.Id ? ref prevContact.Link1 : ref prevContact.Link2;
            link.NextIndex = currentIndex;
        }

        body2.LastContactIndex = currentIndex;
        body2.ContactCount++;
    }
}