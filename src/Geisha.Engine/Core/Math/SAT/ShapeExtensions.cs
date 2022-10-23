using System.Diagnostics;
using System.Linq;

namespace Geisha.Engine.Core.Math.SAT
{
    // Algorithms based on:
    // - http://www.dyn4j.org/2010/01/sat/#sat-curve
    /// <summary>
    ///     Provides a set of static methods to perform geometric operations on objects that implement <see cref="IShape" />.
    /// </summary>
    public static class ShapeExtensions
    {
        /// <summary>
        ///     Tests whether shape contains a point.
        /// </summary>
        /// <param name="shape">Shape to be tested for containing a point.</param>
        /// <param name="point">Point to be tested for containment in a shape.</param>
        /// <returns>True, if shape contains a point, false otherwise.</returns>
        /// <remarks>
        ///     Point is considered to be contained in a shape when it is strictly located inside a shape or it is a part of
        ///     shape's edge
        /// </remarks>
        public static bool Contains(this IShape shape, Vector2 point)
        {
            if (shape.IsCircle) return shape.Center.Distance(point) <= shape.Radius;

            var axes = shape.GetAxes();
            if (axes.Length == 0)
            {
                axes = ComputeAxes(shape);
            }

            for (var i = 0; i < axes.Length; i++)
            {
                var projection1 = axes[i].GetProjectionOf(shape);
                var projection2 = axes[i].GetProjectionOf(point);

                if (!projection1.Overlaps(projection2)) return false;
            }

            return true;
        }

        /// <summary>
        ///     Tests whether two shapes overlaps.
        /// </summary>
        /// <param name="shape1">First shape to be tested for overlapping with the other.</param>
        /// <param name="shape2">Second shape to be tested for overlapping with the other.</param>
        /// <returns>True, if shapes overlap, false otherwise.</returns>
        /// <remarks>
        ///     Two shapes are considered overlapping when they strictly intersect each other or they have a common edge
        ///     points or vertices.
        /// </remarks>
        public static bool Overlaps(this IShape shape1, IShape shape2)
        {
            if (shape1.IsCircle)
            {
                if (shape2.IsCircle) return CirclesOverlap(shape1, shape2);
                return PolygonAndCircleOverlap(shape2, shape1);
            }

            if (shape2.IsCircle) return PolygonAndCircleOverlap(shape1, shape2);
            return PolygonsOverlap(shape1, shape2);
        }

        private static bool CirclesOverlap(IShape circle1, IShape circle2)
        {
            return circle1.Center.Distance(circle2.Center) <= circle1.Radius + circle2.Radius;
        }

        private static bool PolygonsOverlap(IShape polygon1, IShape polygon2)
        {
            var axes1 = polygon1.GetAxes();
            if (axes1.Length == 0)
            {
                axes1 = ComputeAxes(polygon1);
            }

            var axes2 = polygon2.GetAxes();
            if (axes2.Length == 0)
            {
                axes2 = ComputeAxes(polygon2);
            }

            var axes = axes1.Concat(axes2).ToArray();

            for (var i = 0; i < axes.Length; i++)
            {
                var projection1 = axes[i].GetProjectionOf(polygon1);
                var projection2 = axes[i].GetProjectionOf(polygon2);

                if (!projection1.Overlaps(projection2)) return false;
            }

            return true;
        }

        private static bool PolygonAndCircleOverlap(IShape polygon, IShape circle)
        {
            Debug.Assert(polygon.GetVertices().Length > 2,
                $"shape.GetVertices().Length > 2 -- Number of vertices [{polygon.GetVertices().Length}] must be greater than 2.");

            var vertices = polygon.GetVertices();
            var edgesWithNegativeDistanceToCircleCenterCount = 0;

            for (var i = 0; i < vertices.Length; i++)
            {
                if (circle.Contains(vertices[i])) return true;

                var v1 = vertices[(i - 1 + vertices.Length) % vertices.Length];
                var v2 = vertices[i];

                var edge = v1 - v2;
                var edgeAxis = new Axis(edge);

                var edgeProjection = edgeAxis.GetProjectionOf(new[] {v1, v2});
                var circleCenterProjection = edgeAxis.GetProjectionOf(circle.Center);
                if (edgeProjection.Overlaps(circleCenterProjection))
                {
                    var edgeNormalAxis = new Axis(edge.Normal);
                    var edgeProjectionOnNormalAxis = edgeNormalAxis.GetProjectionOf(v1);
                    var circleCenterProjectionOnNormalAxis = edgeNormalAxis.GetProjectionOf(circle.Center);

                    var circleCenterDistanceToEdge = circleCenterProjectionOnNormalAxis.Max - edgeProjectionOnNormalAxis.Max;
                    if (System.Math.Abs(circleCenterDistanceToEdge) <= circle.Radius) return true;

                    // If distance of circle center to edge is positive and bigger than radius then there can be no collision (circle center outside of polygon and far away from certain edge)
                    if (circleCenterDistanceToEdge > 0) return false;
                    if (circleCenterDistanceToEdge < 0) edgesWithNegativeDistanceToCircleCenterCount++;
                }
            }

            // If distance of circle center to each edge is negative then circle center is inside a polygon
            return edgesWithNegativeDistanceToCircleCenterCount == vertices.Length;
        }

        private static Axis[] ComputeAxes(IShape shape)
        {
            Debug.Assert(shape.GetVertices().Length > 2,
                $"shape.GetVertices().Length > 2 -- Number of vertices [{shape.GetVertices().Length}] must be greater than 2.");

            var vertices = shape.GetVertices();
            var axes = new Axis[vertices.Length];

            axes[0] = new Axis((vertices[^1] - vertices[0]).Normal);
            for (var i = 1; i < vertices.Length; i++)
            {
                var edge = vertices[i - 1] - vertices[i];
                axes[i] = new Axis(edge.Normal);
            }

            return axes;
        }
    }
}