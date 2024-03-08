using Geisha.Engine.Core.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

// TODO Add documentation?
public readonly struct ContactPoint
{
    public ContactPoint(in Vector2 worldPosition, in Vector2 localPositionA, in Vector2 localPositionB, in Vector2 collisionNormal, double separationDepth)
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
    public Contact(RigidBody2D body1, RigidBody2D body2, in ContactPoint point) : this()
    {
        Body1 = body1;
        Body2 = body2;
        Point1 = point;
        Point2 = default;
        PointsCount = 1;
    }

    public Contact(RigidBody2D body1, RigidBody2D body2, in ContactPoint point1, in ContactPoint point2)
    {
        Body1 = body1;
        Body2 = body2;
        Point1 = point1;
        Point2 = point2;
        PointsCount = 2;
    }

    public RigidBody2D Body1 { get; }
    public RigidBody2D Body2 { get; }
    public ContactPoint Point1 { get; }
    public ContactPoint Point2 { get; }
    public int PointsCount { get; }

    public ContactPoint PointAt(int index) => index switch
    {
        0 => Point1,
        1 => Point2,
        _ => throw new ArgumentOutOfRangeException(nameof(index), index, "Valid range is [0,1].")
    };
}

internal static class ContactGenerator
{
    public static Contact GenerateContact(RigidBody2D body1, RigidBody2D body2, in SeparationInfo separationInfo)
    {
        if (body1.IsCircleCollider && body2.IsCircleCollider)
        {
            var contactPoint = GenerateContactForCircleVsCircle(
                body1.TransformedCircleCollider,
                body2.TransformedCircleCollider,
                separationInfo
            );
            return new Contact(body1, body2, contactPoint);
        }

        if (body1.IsRectangleCollider && body2.IsRectangleCollider)
        {
            var (count, p1, p2) = GenerateContactForRectangleVsRectangle(
                body1.TransformedRectangleCollider,
                body2.TransformedRectangleCollider,
                separationInfo
            );

            return count switch
            {
                2 => new Contact(body1, body2, p1, p2),
                1 => new Contact(body1, body2, p1),
                _ => throw new InvalidOperationException("Found zero contact points for rectangles collision.")
            };
        }

        if (body1.IsCircleCollider && body2.IsRectangleCollider)
        {
            var contactPoint = GenerateContactForCircleVsRectangle(
                body1.TransformedCircleCollider,
                body2.TransformedRectangleCollider,
                separationInfo
            );
            return new Contact(body1, body2, contactPoint);
        }

        if (body1.IsRectangleCollider && body2.IsCircleCollider)
        {
            var contactPoint = GenerateContactForRectangleVsCircle(
                body1.TransformedRectangleCollider,
                body2.TransformedCircleCollider,
                separationInfo
            );
            return new Contact(body1, body2, contactPoint);
        }

        throw new InvalidOperationException("Unsupported collider for contact generation.");
    }

    private static ContactPoint GenerateContactForCircleVsCircle(in Circle c1, in Circle c2, in SeparationInfo separationInfo)
    {
        var worldPosition = c1.Center.Midpoint(c2.Center);
        var localPositionA = worldPosition - c1.Center;
        var localPositionB = worldPosition - c2.Center;
        return new ContactPoint(worldPosition, localPositionA, localPositionB, separationInfo.Normal, separationInfo.Depth);
    }

    private static ContactPoint GenerateContactForCircleVsRectangle(in Circle c, in Rectangle r, in SeparationInfo separationInfo)
    {
        var worldPosition = c.Center + separationInfo.Normal.Opposite * (c.Radius - separationInfo.Depth * 0.5);
        var localPositionA = worldPosition - c.Center;
        var localPositionB = worldPosition - r.Center;
        return new ContactPoint(worldPosition, localPositionA, localPositionB, separationInfo.Normal, separationInfo.Depth);
    }

    private static ContactPoint GenerateContactForRectangleVsCircle(in Rectangle r, in Circle c, in SeparationInfo separationInfo)
    {
        var worldPosition = c.Center + separationInfo.Normal * (c.Radius - separationInfo.Depth * 0.5);
        var localPositionA = worldPosition - c.Center;
        var localPositionB = worldPosition - r.Center;
        return new ContactPoint(worldPosition, localPositionA, localPositionB, separationInfo.Normal, separationInfo.Depth);
    }

    private static (int Count, ContactPoint P1, ContactPoint P2) GenerateContactForRectangleVsRectangle(in Rectangle r1, in Rectangle r2,
        in SeparationInfo separationInfo)
    {
        var collisionNormal = separationInfo.Normal;
        Span<Vector2> polygon1 = stackalloc Vector2[4];
        Span<Vector2> polygon2 = stackalloc Vector2[4];
        r1.WriteVertices(polygon1);
        r2.WriteVertices(polygon2);

        // TODO Introduce LineSegment struct?
        Span<Vector2> edge1 = stackalloc Vector2[2];
        Span<Vector2> edge2 = stackalloc Vector2[2];
        FindSignificantEdge(polygon1, collisionNormal, edge1);
        FindSignificantEdge(polygon2, collisionNormal.Opposite, edge2);

        // Dummy assignments to make compiler happy about CS8352.
        // ReSharper disable once RedundantAssignment
        var reference = edge1;
        // ReSharper disable once RedundantAssignment
        var incident = edge2;
        if (collisionNormal.Angle((edge1[1] - edge1[0]).Normal) < collisionNormal.Opposite.Angle((edge2[1] - edge2[0]).Normal))
        {
            reference = edge1;
            incident = edge2;
        }
        else
        {
            reference = edge2;
            incident = edge1;
            collisionNormal = collisionNormal.Opposite;
        }

        Span<Vector2> clipPoints = stackalloc Vector2[2];
        var count = ClipIncidentToReference(incident, reference, collisionNormal, clipPoints);

        ContactPoint p1 = default;
        if (count > 0)
        {
            var worldPosition = clipPoints[0];
            var localPositionA = worldPosition - r1.Center;
            var localPositionB = worldPosition - r2.Center;
            p1 = new ContactPoint(worldPosition, localPositionA, localPositionB, separationInfo.Normal, separationInfo.Depth);
        }

        ContactPoint p2 = default;
        if (count > 1)
        {
            var worldPosition = clipPoints[1];
            var localPositionA = worldPosition - r1.Center;
            var localPositionB = worldPosition - r2.Center;
            p2 = new ContactPoint(worldPosition, localPositionA, localPositionB, separationInfo.Normal, separationInfo.Depth);
        }

        return (count, p1, p2);
    }

    private static void FindSignificantEdge(ReadOnlySpan<Vector2> polygon, in Vector2 collisionNormal, Span<Vector2> foundEdge)
    {
        Debug.Assert(polygon.Length > 2, "polygon.Length > 2");
        Debug.Assert(foundEdge.Length == 2, "foundEdge.Length == 2");

        var axis = new Axis(collisionNormal);

        var min = double.MaxValue;
        var vertexIndex = 0;

        for (var i = 0; i < polygon.Length; i++)
        {
            var v = polygon[i];
            var projection = axis.GetProjectionOf(v);
            if (projection.Min < min)
            {
                min = projection.Min;
                vertexIndex = i;
            }
        }

        var v0 = polygon[(vertexIndex - 1 + polygon.Length) % polygon.Length];
        var v1 = polygon[vertexIndex];
        var v2 = polygon[(vertexIndex + 1 + polygon.Length) % polygon.Length];

        if (collisionNormal.Angle((v1 - v0).Normal) < collisionNormal.Angle((v2 - v1).Normal))
        {
            foundEdge[0] = v0;
            foundEdge[1] = v1;
        }
        else
        {
            foundEdge[0] = v1;
            foundEdge[1] = v2;
        }
    }

    private static int ClipIncidentToReference(ReadOnlySpan<Vector2> incident, ReadOnlySpan<Vector2> reference, in Vector2 collisionNormal,
        Span<Vector2> clipPoints)
    {
        Debug.Assert(incident.Length == 2, "incident.Length == 2");
        Debug.Assert(reference.Length == 2, "reference.Length == 2");
        Debug.Assert(clipPoints.Length == 2, "clipPoints.Length == 2");

        clipPoints[0] = incident[0];
        clipPoints[1] = incident[1];

        var oppositeCollisionNormal = collisionNormal.Opposite;

        var axis = new Axis(reference[1] - reference[0]);
        var referenceProjection = axis.GetProjectionOf(reference);
        var v0Projection = axis.GetProjectionOf(clipPoints[0]);
        var v1Projection = axis.GetProjectionOf(clipPoints[1]);

        if (v0Projection.Max < referenceProjection.Min)
        {
            clipPoints[0] = clipPoints[1] + (clipPoints[0] - clipPoints[1]) *
                ((v1Projection.Max - referenceProjection.Min) / (v1Projection.Max - v0Projection.Max));
        }
        else if (v1Projection.Max < referenceProjection.Min)
        {
            clipPoints[1] = clipPoints[0] + (clipPoints[1] - clipPoints[0]) *
                ((v0Projection.Max - referenceProjection.Min) / (v0Projection.Max - v1Projection.Max));
        }

        axis = new Axis(reference[0] - reference[1]);
        referenceProjection = axis.GetProjectionOf(reference);
        v0Projection = axis.GetProjectionOf(clipPoints[0]);
        v1Projection = axis.GetProjectionOf(clipPoints[1]);

        if (v0Projection.Max < referenceProjection.Min)
        {
            clipPoints[0] = clipPoints[1] + (clipPoints[0] - clipPoints[1]) *
                ((v1Projection.Max - referenceProjection.Min) / (v1Projection.Max - v0Projection.Max));
        }
        else if (v1Projection.Max < referenceProjection.Min)
        {
            clipPoints[1] = clipPoints[0] + (clipPoints[1] - clipPoints[0]) *
                ((v0Projection.Max - referenceProjection.Min) / (v0Projection.Max - v1Projection.Max));
        }

        axis = new Axis(oppositeCollisionNormal);
        referenceProjection = axis.GetProjectionOf(reference[0]);
        var count = 0;
        for (var i = 0; i < clipPoints.Length; i++)
        {
            if (axis.GetProjectionOf(clipPoints[i]).Min <= referenceProjection.Max)
            {
                clipPoints[count++] = clipPoints[i];
            }
        }

        return count;
    }
}

internal static class ContactSolver
{
    public static void SolvePositionConstraints(IReadOnlyList<RigidBody2D> kinematicBodies)
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

            kinematicBody.Position += minimumTranslationVector * 0.9;
            kinematicBody.RecomputeCollider();
        }
    }
}

internal static class PositionConstraint
{
    public static SeparationInfo GetSeparationInfo(in Contact contact)
    {
        if (contact.Body1.IsCircleCollider && contact.Body2.IsCircleCollider)
        {
            contact.Body1.TransformedCircleCollider.Overlaps(contact.Body2.TransformedCircleCollider, out var separationInfo);
            return separationInfo;
        }

        if (contact.Body1.IsCircleCollider && contact.Body2.IsRectangleCollider)
        {
            contact.Body1.TransformedCircleCollider.Overlaps(contact.Body2.TransformedRectangleCollider, out var separationInfo);
            return separationInfo;
        }

        if (contact.Body1.IsRectangleCollider && contact.Body2.IsCircleCollider)
        {
            contact.Body1.TransformedRectangleCollider.Overlaps(contact.Body2.TransformedCircleCollider, out var separationInfo);
            return separationInfo;
        }

        if (contact.Body1.IsRectangleCollider && contact.Body2.IsRectangleCollider)
        {
            contact.Body1.TransformedRectangleCollider.Overlaps(contact.Body2.TransformedRectangleCollider, out var separationInfo);
            return separationInfo;
        }

        return new SeparationInfo(Vector2.Zero, 0);
    }
}