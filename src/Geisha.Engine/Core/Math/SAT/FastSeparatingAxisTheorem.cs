using System;
using System.Linq;

namespace Geisha.Engine.Core.Math.SAT
{
    // TODO Replace ShapeExtensions with usage of this class
    internal static class FastSeparatingAxisTheorem
    {
        public static bool PolygonContains(ReadOnlySpan<Vector2> vertices, in Vector2 point, ReadOnlySpan<Axis> axes)
        {
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
            for (var i = 0; i < axes.Length; i++)
            {
                var projection1 = axes[i].GetProjectionOf(polygon1);
                var projection2 = axes[i].GetProjectionOf(polygon2);

                if (!projection1.Overlaps(projection2)) return false;
            }

            return true;
        }
    }
}