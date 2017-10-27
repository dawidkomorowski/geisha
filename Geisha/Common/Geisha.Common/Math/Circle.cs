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
            return Center.Distance(other.Center) < Radius + other.Radius;
        }
    }
}