using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;
using System;
using System.Collections.Generic;

namespace Geisha.Engine.Physics.Systems;

// TODO Add documentation?
public readonly struct ContactPoint
{
    public ContactPoint(Vector2 worldPosition, Vector2 localPositionA, Vector2 localPositionB, Vector2 collisionNormal, double separationDepth)
    {
        WorldPosition = worldPosition;
        LocalPositionA = localPositionA;
        LocalPositionB = localPositionB;
        CollisionNormal = collisionNormal;
        SeparationDepth = separationDepth;
    }

    public Vector2 WorldPosition { get; }
    public Vector2 LocalPositionA { get; }
    public Vector2 LocalPositionB { get; }
    public Vector2 CollisionNormal { get; }
    public double SeparationDepth { get; }
}

internal readonly struct Contact
{
    public Contact(KinematicBody body1, StaticBody body2, ContactPoint point)
    {
        Body1 = body1;
        Body2 = body2;
        Point = point;
    }

    public KinematicBody Body1 { get; }
    public StaticBody Body2 { get; } // TODO How to uniformly represent static and kinematic bodies?
    public ContactPoint Point { get; }
}

internal static class ContactGenerator
{
    public static ContactPoint GenerateContactForCircleVsCircle(in Circle c1, in Circle c2, in SeparationInfo separationInfo)
    {
        var worldPosition = c1.Center.Midpoint(c2.Center);
        var localPositionA = worldPosition - c1.Center;
        var localPositionB = worldPosition - c2.Center;
        return new ContactPoint(worldPosition, localPositionA, localPositionB, separationInfo.Normal, separationInfo.Depth);
    }
}

internal static class ContactSolver
{
    public static void Solve(IReadOnlyList<KinematicBody> kinematicBodies)
    {
        for (var i = 0; i < kinematicBodies.Count; i++)
        {
            var kinematicBody = kinematicBodies[i];

            var minimumTranslationVector = Vector2.Zero;

            for (int j = 0; j < kinematicBody.Contacts.Count; j++)
            {
                var contact = kinematicBody.Contacts[j];

                var separationInfo = PositionConstraint.GetSeparationInfo(contact);

                if (separationInfo.Depth <= 0.5)
                {
                    continue;
                }

                var localMtv = separationInfo.Normal * separationInfo.Depth;
                if (localMtv.X * minimumTranslationVector.X > 0)
                {
                    if (Math.Abs(localMtv.X) > Math.Abs(minimumTranslationVector.X))
                    {
                        minimumTranslationVector = minimumTranslationVector.WithX(localMtv.X);
                    }
                }
                else
                {
                    minimumTranslationVector = minimumTranslationVector.WithX(minimumTranslationVector.X + localMtv.X);
                }

                if (localMtv.Y * minimumTranslationVector.Y > 0)
                {
                    if (Math.Abs(localMtv.Y) > Math.Abs(minimumTranslationVector.Y))
                    {
                        minimumTranslationVector = minimumTranslationVector.WithY(localMtv.Y);
                    }
                }
                else
                {
                    minimumTranslationVector = minimumTranslationVector.WithY(minimumTranslationVector.Y + localMtv.Y);
                }
            }

            kinematicBody.Position += minimumTranslationVector;
            kinematicBody.UpdateTransform();
        }
    }
}

internal static class PositionConstraint
{
    public static SeparationInfo GetSeparationInfo(Contact contact)
    {
        if (contact.Body1.IsCircleCollider && contact.Body2.IsCircleCollider)
        {
            contact.Body1.TransformedCircle.Overlaps(contact.Body2.TransformedCircle, out var separationInfo);
            return separationInfo;
        }
        else
        {
            return new SeparationInfo(Vector2.Zero, 0);
        }
    }
}