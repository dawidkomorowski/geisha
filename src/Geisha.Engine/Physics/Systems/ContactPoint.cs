using Geisha.Engine.Core.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Geisha.Engine.Physics.Systems;

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
    public Contact(KinematicBody body1, StaticBody body2, in ContactPoint point) : this()
    {
        Body1 = body1;
        Body2 = body2;
        Point1 = point;
        Point2 = default;
        PointsCount = 1;
    }

    public Contact(KinematicBody body1, StaticBody body2, in ContactPoint point1, in ContactPoint point2)
    {
        Body1 = body1;
        Body2 = body2;
        Point1 = point1;
        Point2 = point2;
        PointsCount = 2;
    }

    public KinematicBody Body1 { get; }
    public StaticBody Body2 { get; } // TODO How to uniformly represent static and kinematic bodies?
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
    public static Contact GenerateContact(KinematicBody body1, StaticBody body2, in SeparationInfo separationInfo)
    {
        if (body1.IsRectangleCollider && body2.IsRectangleCollider)
        {
            var (p1, p2) = GenerateContactForRectangleVsRectangle(
                body1.TransformedRectangle,
                body2.TransformedRectangle,
                separationInfo
            );

            return new Contact(body1, body2, p1, p2);
        }

        throw new NotImplementedException();
    }

    public static ContactPoint GenerateContactForCircleVsCircle(in Circle c1, in Circle c2, in SeparationInfo separationInfo)
    {
        var worldPosition = c1.Center.Midpoint(c2.Center);
        var localPositionA = worldPosition - c1.Center;
        var localPositionB = worldPosition - c2.Center;
        return new ContactPoint(worldPosition, localPositionA, localPositionB, separationInfo.Normal, separationInfo.Depth);
    }

    public static ContactPoint GenerateContactForCircleVsRectangle(in Circle c, in Rectangle r, in SeparationInfo separationInfo)
    {
        var worldPosition = c.Center + separationInfo.Normal.Opposite * (c.Radius - separationInfo.Depth * 0.5);
        var localPositionA = worldPosition - c.Center;
        var localPositionB = worldPosition - r.Center;
        return new ContactPoint(worldPosition, localPositionA, localPositionB, separationInfo.Normal, separationInfo.Depth);
    }

    public static ContactPoint GenerateContactForRectangleVsCircle(in Rectangle r, in Circle c, in SeparationInfo separationInfo)
    {
        var worldPosition = c.Center + separationInfo.Normal * (c.Radius - separationInfo.Depth * 0.5);
        var localPositionA = worldPosition - c.Center;
        var localPositionB = worldPosition - r.Center;
        return new ContactPoint(worldPosition, localPositionA, localPositionB, separationInfo.Normal, separationInfo.Depth);
    }

    private static (ContactPoint P1, ContactPoint P2) GenerateContactForRectangleVsRectangle(in Rectangle r1, in Rectangle r2, in SeparationInfo separationInfo)
    {
        // TODO This is fake calculation just for temporary debugging.
        //var worldPosition = r1.Center + separationInfo.Normal.Opposite * (r1.Height * 0.5 - separationInfo.Depth * 0.5);

        Span<Vector2> polygon1 = stackalloc Vector2[4];
        Span<Vector2> polygon2 = stackalloc Vector2[4];
        r1.WriteVertices(polygon1);
        r2.WriteVertices(polygon2);

        Span<Vector2> edge1 = stackalloc Vector2[2];
        Span<Vector2> edge2 = stackalloc Vector2[2];
        FindSignificantEdge(polygon1, separationInfo.Normal, edge1);
        FindSignificantEdge(polygon2, separationInfo.Normal.Opposite, edge2);

        var worldPosition = edge1[0];
        var localPositionA = worldPosition - r1.Center;
        var localPositionB = worldPosition - r2.Center;
        var p1 = new ContactPoint(worldPosition, localPositionA, localPositionB, separationInfo.Normal, separationInfo.Depth);

        worldPosition = edge1[1];
        localPositionA = worldPosition - r1.Center;
        localPositionB = worldPosition - r2.Center;
        var p2 = new ContactPoint(worldPosition, localPositionA, localPositionB, separationInfo.Normal, separationInfo.Depth);
        return (p1, p2);
    }

    private static void FindSignificantEdge(ReadOnlySpan<Vector2> polygon, in Vector2 collisionNormal, Span<Vector2> edge)
    {
        Debug.Assert(polygon.Length > 2, "polygon.Length > 2");
        Debug.Assert(edge.Length == 2, "edge.Length == 2");

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

        if (collisionNormal.Angle(v1 - v0) < collisionNormal.Angle(v2 - v1))
        {
            edge[0] = v0;
            edge[1] = v1;
        }
        else
        {
            edge[0] = v1;
            edge[1] = v2;
        }
    }
}

internal static class ContactSolver
{
    public static void SolvePositionConstraints(IReadOnlyList<KinematicBody> kinematicBodies)
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
    public static SeparationInfo GetSeparationInfo(in Contact contact)
    {
        if (contact.Body1.IsCircleCollider && contact.Body2.IsCircleCollider)
        {
            contact.Body1.TransformedCircle.Overlaps(contact.Body2.TransformedCircle, out var separationInfo);
            return separationInfo;
        }

        if (contact.Body1.IsCircleCollider && contact.Body2.IsRectangleCollider)
        {
            contact.Body1.TransformedCircle.Overlaps(contact.Body2.TransformedRectangle, out var separationInfo);
            return separationInfo;
        }

        if (contact.Body1.IsRectangleCollider && contact.Body2.IsCircleCollider)
        {
            contact.Body1.TransformedRectangle.Overlaps(contact.Body2.TransformedCircle, out var separationInfo);
            return separationInfo;
        }

        if (contact.Body1.IsRectangleCollider && contact.Body2.IsRectangleCollider)
        {
            contact.Body1.TransformedRectangle.Overlaps(contact.Body2.TransformedRectangle, out var separationInfo);
            return separationInfo;
        }

        return new SeparationInfo(Vector2.Zero, 0);
    }
}