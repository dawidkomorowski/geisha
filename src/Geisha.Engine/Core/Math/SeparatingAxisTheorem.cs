using System;
using System.Diagnostics;

namespace Geisha.Engine.Core.Math
{
    internal static class SeparatingAxisTheorem
    {
        public static bool PolygonContains(ReadOnlySpan<Vector2> vertices, in Vector2 point, ReadOnlySpan<Axis> axes)
        {
            Debug.Assert(vertices.Length > 2, "vertices.Length > 2");

            for (var i = 0; i < axes.Length; i++)
            {
                var projection1 = axes[i].GetProjectionOf(vertices);
                var projection2 = axes[i].GetProjectionOf(point);

                if (!projection1.Overlaps(projection2)) return false;
            }

            return true;
        }

        public static bool PolygonsOverlap(ReadOnlySpan<Vector2> polygon1, ReadOnlySpan<Vector2> polygon2, ReadOnlySpan<Axis> axes)
        {
            Debug.Assert(polygon1.Length > 2, "polygon1.Length > 2");
            Debug.Assert(polygon2.Length > 2, "polygon2.Length > 2");

            for (var i = 0; i < axes.Length; i++)
            {
                var projection1 = axes[i].GetProjectionOf(polygon1);
                var projection2 = axes[i].GetProjectionOf(polygon2);

                if (!projection1.Overlaps(projection2)) return false;
            }

            return true;
        }

        public static bool PolygonsOverlap(ReadOnlySpan<Vector2> polygon1, ReadOnlySpan<Vector2> polygon2, ReadOnlySpan<Axis> axes, out SeparationInfo separationInfo)
        {
            Debug.Assert(polygon1.Length > 2, "polygon1.Length > 2");
            Debug.Assert(polygon2.Length > 2, "polygon2.Length > 2");

            separationInfo = new SeparationInfo(Vector2.Zero, 0);

            for (var i = 0; i < axes.Length; i++)
            {
                var projection1 = axes[i].GetProjectionOf(polygon1);
                var projection2 = axes[i].GetProjectionOf(polygon2);

                if (!projection1.Overlaps(projection2)) return false;
            }

            return true;
        }

        public static bool PolygonAndCircleOverlap(ReadOnlySpan<Vector2> vertices, in Circle circle)
        {
            Debug.Assert(vertices.Length > 2, "vertices.Length > 2");

            var edgesWithNegativeDistanceToCircleCenterCount = 0;

            Span<Vector2> edgeVertices = stackalloc Vector2[2];

            for (var i = 0; i < vertices.Length; i++)
            {
                if (circle.Contains(vertices[i])) return true;

                var v1 = vertices[(i - 1 + vertices.Length) % vertices.Length];
                var v2 = vertices[i];
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
            return edgesWithNegativeDistanceToCircleCenterCount == vertices.Length;
        }

        public static bool PolygonAndCircleOverlap(ReadOnlySpan<Vector2> vertices, in Circle circle, out SeparationInfo separationInfo)
        {
            Debug.Assert(vertices.Length > 2, "vertices.Length > 2");

            separationInfo = new SeparationInfo(Vector2.Zero, 0);
            var minSeparationInfo = new SeparationInfo(Vector2.Zero, double.MaxValue);

            var testVertices = true;

            Span<Vector2> edgeVertices = stackalloc Vector2[2];

            for (var i = 0; i < vertices.Length; i++)
            {
                var v1 = vertices[(i - 1 + vertices.Length) % vertices.Length];
                var v2 = vertices[i];
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

                for (var i = 0; i < vertices.Length; i++)
                {
                    var vertex = vertices[i];
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