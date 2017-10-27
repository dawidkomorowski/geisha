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

        public Projection Project(IShape shape)
        {
            var vertices = shape.GetVertices();
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
    }
}