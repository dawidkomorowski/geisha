using System;
using System.Diagnostics;
using System.Linq;

namespace Geisha.Common.Math.SAT
{
    // TODO add documentation
    public static class ShapeExtensions
    {
        public static bool Contains(this IShape shape, Vector2 point)
        {
            if (shape.IsCircle)
            {
                return shape.Center.Distance(point) <= shape.Radius;
            }
            else
            {
                var axes = shape.GetAxes() ?? ComputeAxes(shape);

                for (var i = 0; i < axes.Length; i++)
                {
                    var projection1 = axes[i].GetProjectionOf(shape);
                    var projection2 = axes[i].GetProjectionOf(point);

                    if (!projection1.Overlaps(projection2)) return false;
                }

                return true;
            }
        }

        public static bool Overlaps(this IShape shape1, IShape shape2)
        {
            if (shape1.IsCircle)
            {
                if (shape2.IsCircle)
                {
                    return CirclesOverlap(shape1, shape2);
                }

                return PolygonAndCircleOverlap(shape2, shape1);
            }
            else if (shape2.IsCircle)
            {
                return PolygonAndCircleOverlap(shape1, shape2);
            }

            return PolygonsOverlap(shape1, shape2);
        }

        private static bool CirclesOverlap(IShape circle1, IShape circle2)
        {
            return circle1.Center.Distance(circle2.Center) <= circle1.Radius + circle2.Radius;
        }

        private static bool PolygonsOverlap(IShape polygon1, IShape polygon2)
        {
            var axes1 = polygon1.GetAxes() ?? ComputeAxes(polygon1);
            var axes2 = polygon2.GetAxes() ?? ComputeAxes(polygon2);

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

            for (var i = 0; i < vertices.Length; i++)
            {
                if (circle.Contains(vertices[i])) return true;
            }

            throw new NotImplementedException();
        }

        private static Axis[] ComputeAxes(IShape shape)
        {
            Debug.Assert(shape.GetVertices().Length > 2,
                $"shape.GetVertices().Length > 2 -- Number of vertices [{shape.GetVertices().Length}] must be greater than 2.");

            var vertices = shape.GetVertices();
            var axes = new Axis[vertices.Length];

            axes[0] = new Axis((vertices[vertices.Length - 1] - vertices[0]).Normal);
            for (var i = 1; i < vertices.Length; i++)
            {
                var edge = vertices[i - 1] - vertices[i];
                axes[i] = new Axis(edge.Normal);
            }

            return axes;
        }
    }
}