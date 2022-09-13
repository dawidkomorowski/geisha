using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace Geisha.Engine.Rendering.DirectX
{
    internal static class EllipseExtensions
    {
        public static Ellipse ToDirectXEllipse(this Core.Math.Ellipse ellipse)
        {
            return new Ellipse(new RawVector2((float) ellipse.Center.X, (float) -ellipse.Center.Y), (float) ellipse.RadiusX,
                (float) ellipse.RadiusY);
        }
    }
}