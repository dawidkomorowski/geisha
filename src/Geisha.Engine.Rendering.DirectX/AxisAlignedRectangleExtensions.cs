using Geisha.Common.Math;
using SharpDX.Mathematics.Interop;

namespace Geisha.Engine.Rendering.DirectX
{
    internal static class AxisAlignedRectangleExtensions
    {
        public static RawRectangleF ToRawRectangleF(this in AxisAlignedRectangle rectangle)
        {
            return new RawRectangleF((float)rectangle.UpperLeft.X, (float)-rectangle.UpperLeft.Y,
                (float)rectangle.LowerRight.X, (float)-rectangle.LowerRight.Y);
        }
    }
}