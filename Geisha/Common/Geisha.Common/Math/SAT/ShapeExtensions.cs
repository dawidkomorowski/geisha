using System.Diagnostics;
using System.Linq;

namespace Geisha.Common.Math.SAT
{
    // TODO add documentation
    public static class ShapeExtensions
    {
        public static bool Overlaps(this IShape shape1, IShape shape2)
        {
            if (shape1.IsCircle && shape2.IsCircle)
                return shape1.Center.Distance(shape2.Center) < shape1.Radius + shape2.Radius;

            var axes1 = shape1.GetAxes() ?? ComputeAxes(shape1);
            var axes2 = shape2.GetAxes() ?? ComputeAxes(shape2);

            var axes = axes1.Concat(axes2).ToArray();

            for (var i = 0; i < axes.Length; i++)
            {
                var projection1 = axes[i].Project(shape1);
                var projection2 = axes[i].Project(shape2);

                if (!projection1.Overlaps(projection2)) return false;
            }

            return true;
        }

        private static Axis[] ComputeAxes(IShape shape)
        {
            if (shape.IsCircle) return new Axis[0];

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