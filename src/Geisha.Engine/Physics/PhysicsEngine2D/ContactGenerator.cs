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
                body2.TransformedCircleCollider,
                mtv
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

    private static ContactPoint GenerateContactPointForCircleVsCircle(in Circle c1, in Circle c2, in MinimumTranslationVector mtv)
    {
        var worldPosition = c1.Center + mtv.Direction.Opposite * (c1.Radius - mtv.Length * 0.5);
        var localPosition1 = worldPosition - c1.Center;
        var localPosition2 = worldPosition - c2.Center;
        return new ContactPoint(worldPosition, localPosition1, localPosition2);
    }

    private static ContactPoint GenerateContactPointForCircleVsRectangle(in Circle c, in Rectangle r, in MinimumTranslationVector mtv)
    {
        var worldPosition = c.Center + mtv.Direction.Opposite * (c.Radius - mtv.Length * 0.5);
        var localPosition1 = worldPosition - c.Center;
        var localPosition2 = worldPosition - r.Center;
        return new ContactPoint(worldPosition, localPosition1, localPosition2);
    }

    private static ContactPoint GenerateContactPointForRectangleVsCircle(in Rectangle r, in Circle c, in MinimumTranslationVector mtv)
    {
        var worldPosition = c.Center + mtv.Direction * (c.Radius - mtv.Length * 0.5);
        var localPosition1 = worldPosition - r.Center;
        var localPosition2 = worldPosition - c.Center;
        return new ContactPoint(worldPosition, localPosition1, localPosition2);
    }

    private static ReadOnlyFixedList2<ContactPoint> GenerateContactPointsForRectangleVsRectangle(in Rectangle r1, in Rectangle r2,
        in MinimumTranslationVector mtv)
    {
        var collisionNormal = mtv.Direction;
        Span<Vector2> polygon1 = stackalloc Vector2[4];
        Span<Vector2> polygon2 = stackalloc Vector2[4];
        r1.WriteVertices(polygon1);
        r2.WriteVertices(polygon2);

        var edge1 = FindSignificantEdge(polygon1, collisionNormal);
        var edge2 = FindSignificantEdge(polygon2, collisionNormal.Opposite);

        LineSegment reference;
        LineSegment incident;
        if (collisionNormal.Angle(edge1.Normal) < collisionNormal.Opposite.Angle(edge2.Normal))
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
            var localPosition1 = worldPosition - r1.Center;
            var localPosition2 = worldPosition - r2.Center;
            var cp = new ContactPoint(worldPosition, localPosition1, localPosition2);
            contactPoints.Add(cp);
        }

        if (count > 1)
        {
            var worldPosition = clipPoints[1];
            var localPosition1 = worldPosition - r1.Center;
            var localPosition2 = worldPosition - r2.Center;
            var cp = new ContactPoint(worldPosition, localPosition1, localPosition2);
            contactPoints.Add(cp);
        }

        Debug.Assert(contactPoints.Count > 0, "contactPoints.Count > 0");

        return contactPoints.ToReadOnly();
    }

    private static LineSegment FindSignificantEdge(ReadOnlySpan<Vector2> polygon, in Vector2 collisionNormal)
    {
        Polygon2D.DebugAssert_PolygonIsOrientedCounterClockwise(polygon);

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
            return new LineSegment(v0, v1);
        }
        else
        {
            return new LineSegment(v1, v2);
        }
    }

    /// <summary>
    ///     This method is based on the Sutherland-Hodgman algorithm. It assumes <paramref name="reference" /> is an edge of a
    ///     rectangle so clipping axes are perpendicular to the edge. It assumes the edges are part of polygons oriented
    ///     counter-clockwise.
    /// </summary>
    private static int ClipIncidentToReference(in LineSegment incident, in LineSegment reference, Span<Vector2> clipPoints)
    {
        Debug.Assert(clipPoints.Length == 2, "clipPoints.Length == 2");

        clipPoints[0] = incident.StartPoint;
        clipPoints[1] = incident.EndPoint;

        var axis = new Axis(reference.StartPoint - reference.EndPoint);
        var referenceProjection = axis.GetProjectionOf(reference);
        var v0Projection = axis.GetProjectionOf(clipPoints[0]);
        var v1Projection = axis.GetProjectionOf(clipPoints[1]);

        Debug.Assert(v0Projection.Max > referenceProjection.Min || v1Projection.Max > referenceProjection.Min, "Incident out of clipping region.");
        Debug.Assert(v0Projection.Min < referenceProjection.Max || v1Projection.Min < referenceProjection.Max, "Incident out of clipping region.");
        Debug.Assert(v0Projection.Min < v1Projection.Min, "v0Projection.Min < v1Projection.Min");

        if (v0Projection.Max < referenceProjection.Min)
        {
            clipPoints[0] = incident.EndPoint + (incident.StartPoint - incident.EndPoint) *
                ((v1Projection.Max - referenceProjection.Min) / (v1Projection.Max - v0Projection.Max));
        }

        if (v1Projection.Min > referenceProjection.Max)
        {
            clipPoints[1] = incident.StartPoint + (incident.EndPoint - incident.StartPoint) *
                ((referenceProjection.Max - v0Projection.Min) / (v1Projection.Max - v0Projection.Max));
        }

        axis = new Axis(reference.Normal.Opposite);
        referenceProjection = axis.GetProjectionOf(reference.StartPoint);
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