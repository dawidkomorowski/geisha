namespace Geisha.Engine.Core.Math;

// TODO Add documentation.
// TODO Add tests.
// TODO Implement equality and formatting.
public readonly struct LineSegment
{
    public LineSegment(in Vector2 startPoint, in Vector2 endPoint)
    {
        StartPoint = startPoint;
        EndPoint = endPoint;
    }

    public Vector2 StartPoint { get; }
    public Vector2 EndPoint { get; }

    public enum IntersectionResult
    {
        NoIntersection,
        LineIntersection,
        LineSegmentIntersection
    }

    public IntersectionResult Intersects(in LineSegment other, out Vector2 intersectionPoint)
    {
        var thisVector = EndPoint - StartPoint;
        var otherVector = other.EndPoint - other.StartPoint;

        var vectorCross = thisVector.Cross(otherVector);

        // Parallel lines have no intersection.
        if (vectorCross == 0)
        {
            intersectionPoint = default;
            return IntersectionResult.NoIntersection;
        }

        // Use parametric equation to describe a line:
        // L(t) = s + t*v
        // L(t) - line point for parameter t
        // s - start point of line segment
        // v - vector from start point to end point of line segment; v = e - s; e - end point
        // t - parameter; in range [0,1] it describes line segment; for all values it describes a line
        // Solve L1(t) = L2(u) and then use parametric equation to find intersection point.

        var startPointDiff = other.StartPoint - StartPoint;
        var thisParam = startPointDiff.Cross(otherVector) / vectorCross;
        var otherParam = startPointDiff.Cross(thisVector) / vectorCross;

        intersectionPoint = StartPoint + thisVector * thisParam;

        if (thisParam is >= 0 and <= 1 && otherParam is >= 0 and <= 1)
        {
            return IntersectionResult.LineSegmentIntersection;
        }

        return IntersectionResult.LineIntersection;
    }
}