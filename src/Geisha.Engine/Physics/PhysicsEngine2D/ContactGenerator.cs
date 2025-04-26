using System;
using System.Diagnostics;
using Geisha.Engine.Core.Collections;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal static class ContactGenerator
{
    public static Contact GenerateContact(RigidBody2D body1, RigidBody2D body2, in MinimumTranslationVector mtv)
    {
        if (body1.IsCircleCollider && body2.IsCircleCollider)
        {
            var contactPoint = GenerateContactPointForCircleVsCircle(
                body1.TransformedCircleCollider,
                body2.TransformedCircleCollider
            );
            return new Contact(body1, body2, mtv.Direction, mtv.Length, new ReadOnlyFixedList2<ContactPoint>(contactPoint));
        }

        if (body1.IsRectangleCollider && body2.IsRectangleCollider)
        {
            var contactPoints = GenerateContactPointsForRectangleVsRectangle(
                body1.TransformedRectangleCollider,
                body2.TransformedRectangleCollider,
                mtv
            );
            return new Contact(body1, body2, mtv.Direction, mtv.Length, contactPoints);
        }

        if (body1.IsCircleCollider && body2.IsRectangleCollider)
        {
            var contactPoint = GenerateContactPointForCircleVsRectangle(
                body1.TransformedCircleCollider,
                body2.TransformedRectangleCollider,
                mtv
            );
            return new Contact(body1, body2, mtv.Direction, mtv.Length, new ReadOnlyFixedList2<ContactPoint>(contactPoint));
        }

        if (body1.IsRectangleCollider && body2.IsCircleCollider)
        {
            var contactPoint = GenerateContactPointForRectangleVsCircle(
                body1.TransformedRectangleCollider,
                body2.TransformedCircleCollider,
                mtv
            );
            return new Contact(body1, body2, mtv.Direction, mtv.Length, new ReadOnlyFixedList2<ContactPoint>(contactPoint));
        }

        throw new InvalidOperationException("Unsupported collider for contact generation.");
    }

    private static ContactPoint GenerateContactPointForCircleVsCircle(in Circle c1, in Circle c2)
    {
        var worldPosition = c1.Center.Midpoint(c2.Center); // TODO This contact is incorrect. Consider circles of different sizes.
        var localPositionA = worldPosition - c1.Center;
        var localPositionB = worldPosition - c2.Center;
        return new ContactPoint(worldPosition, localPositionA, localPositionB);
    }

    private static ContactPoint GenerateContactPointForCircleVsRectangle(in Circle c, in Rectangle r, in MinimumTranslationVector mtv)
    {
        var worldPosition = c.Center + mtv.Direction.Opposite * (c.Radius - mtv.Length * 0.5);
        var localPositionA = worldPosition - c.Center;
        var localPositionB = worldPosition - r.Center;
        return new ContactPoint(worldPosition, localPositionA, localPositionB);
    }

    private static ContactPoint GenerateContactPointForRectangleVsCircle(in Rectangle r, in Circle c, in MinimumTranslationVector mtv)
    {
        var worldPosition = c.Center + mtv.Direction.Normal * (c.Radius - mtv.Length * 0.5);
        var localPositionA = worldPosition - c.Center;
        var localPositionB = worldPosition - r.Center; // TODO Is it swapped? Compare with method above.
        return new ContactPoint(worldPosition, localPositionA, localPositionB);
    }

    private static ReadOnlyFixedList2<ContactPoint> GenerateContactPointsForRectangleVsRectangle(in Rectangle r1, in Rectangle r2,
        in MinimumTranslationVector mtv)
    {
        var collisionNormal = mtv.Direction;
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
        }

        Span<Vector2> clipPoints = stackalloc Vector2[2];
        var count = ClipIncidentToReference(incident, reference, clipPoints);

        FixedList2<ContactPoint> contactPoints = default;
        if (count > 0)
        {
            var worldPosition = clipPoints[0];
            var localPositionA = worldPosition - r1.Center;
            var localPositionB = worldPosition - r2.Center;
            var cp = new ContactPoint(worldPosition, localPositionA, localPositionB);
            contactPoints.Add(cp);
        }

        if (count > 1)
        {
            var worldPosition = clipPoints[1];
            var localPositionA = worldPosition - r1.Center;
            var localPositionB = worldPosition - r2.Center;
            var cp = new ContactPoint(worldPosition, localPositionA, localPositionB);
            contactPoints.Add(cp);
        }

        Debug.Assert(contactPoints.Count > 0, "contactPoints.Count > 0");

        return contactPoints.ToReadOnly();
    }

    private static void FindSignificantEdge(ReadOnlySpan<Vector2> polygon, in Vector2 collisionNormal, Span<Vector2> foundEdge)
    {
        Polygon2D.DebugAssert_PolygonIsOrientedCounterClockwise(polygon);
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

    /// <summary>
    ///     This method is based on the Sutherland-Hodgman algorithm. It assumes <paramref name="reference" /> is an edge of a
    ///     rectangle so clipping axes are perpendicular to the edge. It assumes the edges are part of polygons oriented
    ///     counter-clockwise.
    /// </summary>
    private static int ClipIncidentToReference(ReadOnlySpan<Vector2> incident, ReadOnlySpan<Vector2> reference, Span<Vector2> clipPoints)
    {
        Debug.Assert(incident.Length == 2, "incident.Length == 2");
        Debug.Assert(reference.Length == 2, "reference.Length == 2");
        Debug.Assert(clipPoints.Length == 2, "clipPoints.Length == 2");

        clipPoints[0] = incident[0];
        clipPoints[1] = incident[1];

        var axis = new Axis(reference[0] - reference[1]);
        var referenceProjection = axis.GetProjectionOf(reference);
        var v0Projection = axis.GetProjectionOf(clipPoints[0]);
        var v1Projection = axis.GetProjectionOf(clipPoints[1]);

        Debug.Assert(v0Projection.Max > referenceProjection.Min || v1Projection.Max > referenceProjection.Min, "Incident out of clipping region.");
        Debug.Assert(v0Projection.Min < referenceProjection.Max || v1Projection.Min < referenceProjection.Max, "Incident out of clipping region.");
        Debug.Assert(v0Projection.Min < v1Projection.Min, "v0Projection.Min < v1Projection.Min");

        if (v0Projection.Max < referenceProjection.Min)
        {
            clipPoints[0] = incident[1] + (incident[0] - incident[1]) *
                ((v1Projection.Max - referenceProjection.Min) / (v1Projection.Max - v0Projection.Max));
        }

        if (v1Projection.Min > referenceProjection.Max)
        {
            clipPoints[1] = incident[0] + (incident[1] - incident[0]) *
                ((referenceProjection.Max - v0Projection.Min) / (v1Projection.Max - v0Projection.Max));
        }

        axis = new Axis((reference[0] - reference[1]).Normal);
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