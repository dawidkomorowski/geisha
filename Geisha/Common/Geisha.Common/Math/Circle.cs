using Geisha.Common.Math.SAT;

namespace Geisha.Common.Math
{
    // TODO Consider changing to struct?
    // TODO add documentation
    public class Circle
    {
        private readonly Vector3 _center;

        public Circle(Vector2 center, double radius)
        {
            _center = center.Homogeneous;
            Radius = radius;
        }

        public Circle(double radius) : this(Vector2.Zero, radius)
        {
        }

        public Vector2 Center => _center.ToVector2();
        public double Radius { get; }

        public Circle Transform(Matrix3 transform)
        {
            return new Circle((transform * _center).ToVector2(), Radius);
        }

        public bool Overlaps(Circle other)
        {
            return AsShape().Overlaps(other.AsShape());
        }

        public IShape AsShape()
        {
            return new CircleForSat(this);
        }

        private class CircleForSat : IShape
        {
            private readonly Circle _circle;

            public CircleForSat(Circle circle)
            {
                _circle = circle;
            }

            public bool IsCircle => true;
            public Vector2 Center => _circle.Center;
            public double Radius => _circle.Radius;

            public Axis[] GetAxes()
            {
                throw new System.NotImplementedException(); // TODO
            }

            public Vector2[] GetVertices()
            {
                throw new System.NotImplementedException(); // TODO
            }
        }
    }
}