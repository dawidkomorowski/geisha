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
            if (shape.IsCircle)
            {
                var projected = shape.Center.Dot(_axisAlignedUnitVector);
                return new Projection(projected - shape.Radius, projected + shape.Radius);
            }
            else
            {
                return GetProjectionOf(shape.GetVertices());
            }
        }

        public Projection GetProjectionOf(Vector2[] vertices)
        {
            var min = double.MaxValue;
            var max = double.MinValue;

            for (var i = 0; i < vertices.Length; i++)
            {
                var projected = vertices[i].Dot(_axisAlignedUnitVector);
                min = System.Math.Min(min, projected);
                max = System.Math.Max(max, projected);
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