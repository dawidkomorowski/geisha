namespace Geisha.Common.Math.SAT
{
    // TODO add documentation
    public struct Axis
    {
        private readonly Vector2 _axisAlignedUnitVector;

        public Axis(Vector2 axisAlignedVector)
        {
            _axisAlignedUnitVector = axisAlignedVector.Unit;
        }

        public Projection GetProjectionOf(IShape shape)
        {
            var min = double.MaxValue;
            var max = double.MinValue;

            if (shape.IsCircle)
            {
                var projected = shape.Center.Dot(_axisAlignedUnitVector);
                min = projected - shape.Radius;
                max = projected + shape.Radius;
            }
            else
            {
                var vertices = shape.GetVertices();

                for (var i = 0; i < vertices.Length; i++)
                {
                    var projected = vertices[i].Dot(_axisAlignedUnitVector);
                    min = System.Math.Min(min, projected);
                    max = System.Math.Max(max, projected);
                }
            }

            return new Projection(min, max);
        }

        public Projection GetProjectionOf(Vector2 point)
        {
            var pointProjection = point.Dot(_axisAlignedUnitVector);
            return new Projection(pointProjection, pointProjection);
        }
    }
}