namespace Geisha.Common.Math.Shape
{
    public class Quad : IPolygon<Quad>
    {
        private readonly Vector3 _v1;
        private readonly Vector3 _v2;
        private readonly Vector3 _v3;
        private readonly Vector3 _v4;

        public Vector2 V1 => _v1.ToVector2();
        public Vector2 V2 => _v2.ToVector2();
        public Vector2 V3 => _v3.ToVector2();
        public Vector2 V4 => _v4.ToVector2();

        public Quad(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v4)
        {
            _v1 = v1.Homogeneous;
            _v2 = v2.Homogeneous;
            _v3 = v3.Homogeneous;
            _v4 = v4.Homogeneous;
        }

        public Quad Transform(Matrix3 transform)
        {
            return new Quad(
                (transform * _v1).ToVector2(),
                (transform * _v2).ToVector2(),
                (transform * _v3).ToVector2(),
                (transform * _v4).ToVector2()
            );
        }
    }
}