using System;
using Geisha.Common.Math.SAT;

namespace Geisha.Common.Math
{
    // TODO Consider changing to struct?
    // TODO add documentation
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

        // TODO Bug -> when rotated this calculation is wrong, it should calculate distance not subtraction
        public double Width => System.Math.Abs(UpperRight.X - UpperLeft.X);
        public double Height => System.Math.Abs(UpperLeft.Y - LowerLeft.Y);

        // TODO Possible bug as above - to be checked.
        public Vector2 Center => new Vector2((UpperRight.X + UpperLeft.X) / 2, (UpperLeft.Y + LowerLeft.Y) / 2);

        public new Rectangle Transform(Matrix3 transform)
        {
            return new Rectangle(base.Transform(transform));
        }

        public bool Overlaps(Rectangle other)
        {
            return AsShape().Overlaps(other.AsShape());
        }

        public IShape AsShape()
        {
            return new RectangleForSat(this);
        }

        private class RectangleForSat : IShape
        {
            private readonly Rectangle _rectangle;

            public RectangleForSat(Rectangle rectangle)
            {
                _rectangle = rectangle;
            }

            public bool IsCircle => false;
            public Vector2 Center => throw new NotSupportedException();
            public double Radius => throw new NotSupportedException();

            public Axis[] GetAxes()
            {
                var normal1 = (_rectangle.UpperLeft - _rectangle.LowerLeft).Normal;
                var normal2 = (_rectangle.UpperRight - _rectangle.UpperLeft).Normal;
                return new[] {new Axis(normal1), new Axis(normal2)};
            }

            public Vector2[] GetVertices()
            {
                return new[] {_rectangle.LowerLeft, _rectangle.LowerRight, _rectangle.UpperRight, _rectangle.UpperLeft};
            }
        }
    }
}