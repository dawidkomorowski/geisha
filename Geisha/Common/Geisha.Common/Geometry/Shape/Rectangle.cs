﻿namespace Geisha.Common.Geometry.Shape
{
    public class Rectangle : Quad
    {
        public Vector2 UpperLeft => V4;
        public Vector2 UpperRight => V3;
        public Vector2 LowerLeft => V1;
        public Vector2 LowerRight => V2;

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

        public new Rectangle Transform(Matrix3 transform)
        {
            return new Rectangle(base.Transform(transform));
        }
    }
}