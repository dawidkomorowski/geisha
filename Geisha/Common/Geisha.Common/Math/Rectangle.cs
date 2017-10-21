namespace Geisha.Common.Math
{
    // TODO Consider changing to struct?
    public class Rectangle : Quad
    {
        public Rectangle(Vector2 dimension) : this(Vector2.Zero, dimension)
        {
        }

        public Rectangle(Vector2 center, Vector2 dimension) : base(
            new Vector2(-dimension.X / 2 + center.X, -dimension.Y / 2 + center.Y),
            new Vector2(dimension.X / 2 + center.X, -dimension.Y / 2 + center.Y),
            new Vector2(dimension.X / 2 + center.X, dimension.Y / 2 + center.Y),
            new Vector2(-dimension.X / 2 + center.X, dimension.Y / 2 + center.Y))
        {
        }

        private Rectangle(Quad quad) : base(quad.V1, quad.V2, quad.V3, quad.V4)
        {
        }

        public Vector2 UpperLeft => V4;
        public Vector2 UpperRight => V3;
        public Vector2 LowerLeft => V1;
        public Vector2 LowerRight => V2;

        public Vector2 Center => new Vector2((UpperRight.X + UpperLeft.X) / 2, (UpperLeft.Y + LowerLeft.Y) / 2);
        public double Width => System.Math.Abs(UpperRight.X - UpperLeft.X);
        public double Height => System.Math.Abs(UpperLeft.Y - LowerLeft.Y);

        public new Rectangle Transform(Matrix3 transform)
        {
            return new Rectangle(base.Transform(transform));
        }

        public bool Overlaps(Rectangle other)
        {
            var normal1 = (UpperLeft - LowerLeft).Normal;
            var normal2 = (UpperRight - UpperLeft).Normal;
            var normal3 = (other.UpperLeft - other.LowerLeft).Normal;
            var normal4 = (other.UpperRight - other.UpperLeft).Normal;

            return false;
        }
    }
}