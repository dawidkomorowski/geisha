using System.Linq;

namespace Geisha.Common.Math.SAT
{
    public static class ShapeExtensions
    {
        public static bool Overlaps(this IShape shape1, IShape shape2)
        {
            var axes = shape1.GetAxes().Concat(shape2.GetAxes()).ToArray();

            for (var i = 0; i < axes.Length; i++)
            {
                var projection1 = axes[i].Project(shape1);
                var projection2 = axes[i].Project(shape2);

                if (!projection1.Overlaps(projection2)) return false;
            }

            return true;
        }
    }
}