using System;

namespace Geisha.Engine.Core.Math
{
    internal static class SeparatingAxisTheorem
    {
        public static bool PolygonContains(ReadOnlySpan<Vector2> polygon, in Vector2 point, ReadOnlySpan<Axis> axes)
        {
            Polygon2D.DebugAssert_PolygonIsOrientedCounterClockwise(polygon);

            for (var i = 0; i < axes.Length; i++)
            {
                var projection1 = axes[i].GetProjectionOf(polygon);
                var projection2 = axes[i].GetProjectionOf(point);

                if (!projection1.Overlaps(projection2)) return false;
            }

            return true;
        }

        public static bool PolygonsOverlap(ReadOnlySpan<Vector2> polygon1, ReadOnlySpan<Vector2> polygon2, ReadOnlySpan<Axis> axes)
        {
            Polygon2D.DebugAssert_PolygonIsOrientedCounterClockwise(polygon1, nameof(polygon1));
            Polygon2D.DebugAssert_PolygonIsOrientedCounterClockwise(polygon2, nameof(polygon2));

            for (var i = 0; i < axes.Length; i++)
            {
                var projection1 = axes[i].GetProjectionOf(polygon1);
                var projection2 = axes[i].GetProjectionOf(polygon2);

                if (!projection1.Overlaps(projection2)) return false;
            }

            return true;
        }

        public static bool PolygonsOverlap(ReadOnlySpan<Vector2> polygon1, ReadOnlySpan<Vector2> polygon2, out SeparationInfo separationInfo)
        {
            Polygon2D.DebugAssert_PolygonIsOrientedCounterClockwise(polygon1, nameof(polygon1));
            Polygon2D.DebugAssert_PolygonIsOrientedCounterClockwise(polygon2, nameof(polygon2));

            separationInfo = new SeparationInfo(Vector2.Zero, 0);

            var separationInfo1 = FindSeparationUsingAxesOfPolygon1(polygon1, polygon2);
            if (separationInfo1.Depth < 0)
            {
                return false;
            }

            var separationInfo2 = FindSeparationUsingAxesOfPolygon1(polygon2, polygon1);
            if (separationInfo2.Depth < 0)
            {
                return false;
            }

            if (separationInfo1.Depth < separationInfo2.Depth)
            {
                separationInfo = separationInfo1;
            }
            else
            {
                separationInfo = new SeparationInfo(separationInfo2.Normal.Opposite, separationInfo2.Depth);
            }

            return true;
        }

        private static SeparationInfo FindSeparationUsingAxesOfPolygon1(ReadOnlySpan<Vector2> polygon1, ReadOnlySpan<Vector2> polygon2)
        {
            Polygon2D.DebugAssert_PolygonIsOrientedCounterClockwise(polygon1, nameof(polygon1));
            Polygon2D.DebugAssert_PolygonIsOrientedCounterClockwise(polygon2, nameof(polygon2));

            var minSeparationInfo = new SeparationInfo(Vector2.Zero, double.MaxValue);

            for (var i = 0; i < polygon1.Length; i++)
            {
                var v1 = polygon1[(i - 1 + polygon1.Length) % polygon1.Length];
                var v2 = polygon1[i];

                var edge = v1 - v2;
                var edgeNormal = edge.Normal;
                var axis = new Axis(edgeNormal);
                var polygon1VertexProjection = axis.GetProjectionOf(v2);
                var polygon2Projection = axis.GetProjectionOf(polygon2);

                var distance = polygon2Projection.Min - polygon1VertexProjection.Max;

                var separationDepth = -distance;
                if (separationDepth < minSeparationInfo.Depth)
                {
                    minSeparationInfo = new SeparationInfo(axis.AxisAlignedUnitVector.Opposite, separationDepth);

                    if (separationDepth < 0)
                    {
                        return minSeparationInfo;
                    }
                }
            }

            return minSeparationInfo;
        }

        public static bool PolygonAndCircleOverlap(ReadOnlySpan<Vector2> polygon, in Circle circle)
        {
            Polygon2D.DebugAssert_PolygonIsOrientedCounterClockwise(polygon);

            var edgesWithNegativeDistanceToCircleCenterCount = 0;

            Span<Vector2> edgeVertices = stackalloc Vector2[2];

            for (var i = 0; i < polygon.Length; i++)
            {
                if (circle.Contains(polygon[i])) return true;

                var v1 = polygon[(i - 1 + polygon.Length) % polygon.Length];
                var v2 = polygon[i];
                edgeVertices[0] = v1;
                edgeVertices[1] = v2;

                var edge = v1 - v2;
                var edgeAxis = new Axis(edge);

                var edgeProjection = edgeAxis.GetProjectionOf(edgeVertices);
                var circleCenterProjection = edgeAxis.GetProjectionOf(circle.Center);

                if (edgeProjection.Overlaps(circleCenterProjection))
                {
                    var edgeNormalAxis = new Axis(edge.Normal);
                    var edgeProjectionOnNormalAxis = edgeNormalAxis.GetProjectionOf(v1);
                    var circleCenterProjectionOnNormalAxis = edgeNormalAxis.GetProjectionOf(circle.Center);

                    var circleCenterDistanceToEdge = circleCenterProjectionOnNormalAxis.Max - edgeProjectionOnNormalAxis.Max;
                    if (System.Math.Abs(circleCenterDistanceToEdge) <= circle.Radius) return true;

                    // If distance of circle center to edge is positive and bigger than radius then there can be no collision (circle center outside of polygon and far away from current edge).
                    if (circleCenterDistanceToEdge > 0) return false;
                    if (circleCenterDistanceToEdge < 0) edgesWithNegativeDistanceToCircleCenterCount++;
                }
            }

            // If distance of circle center to each edge is negative then circle center is inside the polygon.
            return edgesWithNegativeDistanceToCircleCenterCount == polygon.Length;
        }

        public static bool PolygonAndCircleOverlap(ReadOnlySpan<Vector2> polygon, in Circle circle, out SeparationInfo separationInfo)
        {
            Polygon2D.DebugAssert_PolygonIsOrientedCounterClockwise(polygon);

            separationInfo = new SeparationInfo(Vector2.Zero, 0);
            var minSeparationInfo = new SeparationInfo(Vector2.Zero, double.MaxValue);

            var testVertices = true;

            Span<Vector2> edgeVertices = stackalloc Vector2[2];

            for (var i = 0; i < polygon.Length; i++)
            {
                var v1 = polygon[(i - 1 + polygon.Length) % polygon.Length];
                var v2 = polygon[i];
                edgeVertices[0] = v1;
                edgeVertices[1] = v2;

                var edge = v1 - v2;
                var edgeAxis = new Axis(edge);

                var edgeProjection = edgeAxis.GetProjectionOf(edgeVertices);
                var circleCenterProjection = edgeAxis.GetProjectionOf(circle.Center);

                if (edgeProjection.Overlaps(circleCenterProjection))
                {
                    var edgeNormal = edge.Normal;
                    var edgeNormalAxis = new Axis(edgeNormal);
                    var edgeProjectionOnNormalAxis = edgeNormalAxis.GetProjectionOf(v1);
                    var circleCenterProjectionOnNormalAxis = edgeNormalAxis.GetProjectionOf(circle.Center);

                    var circleCenterDistanceToEdge = circleCenterProjectionOnNormalAxis.Max - edgeProjectionOnNormalAxis.Max;

                    // If distance of circle center to edge is positive and bigger than radius then there can be no collision (circle center outside of polygon and far away from current edge).
                    if (circleCenterDistanceToEdge > circle.Radius) return false;

                    var separationDepth = circle.Radius - circleCenterDistanceToEdge;

                    if (separationDepth < minSeparationInfo.Depth)
                    {
                        minSeparationInfo = new SeparationInfo(edgeNormal.Opposite, separationDepth);
                        testVertices = false;
                    }
                }
            }

            if (testVertices)
            {
                var vertexCollisionFound = false;

                for (var i = 0; i < polygon.Length; i++)
                {
                    var vertex = polygon[i];
                    var translation = circle.Center - vertex;
                    var circleDistanceToVertex = translation.Length;

                    var separationDepth = circle.Radius - circleDistanceToVertex;

                    if (separationDepth >= 0)
                    {
                        vertexCollisionFound = true;

                        if (separationDepth < minSeparationInfo.Depth)
                        {
                            minSeparationInfo = new SeparationInfo(translation.Unit.Opposite, separationDepth);
                        }
                    }
                }

                if (!vertexCollisionFound)
                {
                    return false;
                }
            }

            if (minSeparationInfo.Depth >= 0)
            {
                separationInfo = minSeparationInfo;
                return true;
            }

            return false;
        }
    }
}