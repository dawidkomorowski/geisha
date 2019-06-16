using Geisha.Common.Math;
using SharpDX.Mathematics.Interop;

namespace Geisha.Framework.Rendering.DirectX
{
    public static class RectangleExtensions
    {
        public static RawRectangleF ToRawRectangleF(this Rectangle rectangle)
        {
            return new RawRectangleF((float) rectangle.UpperLeft.X, (float) -rectangle.UpperLeft.Y,
                (float) rectangle.LowerRight.X, (float) -rectangle.LowerRight.Y);
        }
    }
}