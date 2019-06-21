using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Geisha.Framework.Rendering.DirectX
{
    public static class EllipseExtensions
    {
        public static Ellipse ToDirectXEllipse(this Common.Math.Ellipse ellipse)
        {
            return new Ellipse(new RawVector2((float) ellipse.Center.X, (float) ellipse.Center.Y), (float) ellipse.RadiusX,
                (float) ellipse.RadiusY);
        }
    }
}