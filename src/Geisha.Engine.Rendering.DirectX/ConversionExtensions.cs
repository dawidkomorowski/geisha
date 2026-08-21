using Geisha.Engine.Core.Math;
using SharpDX;
using SharpDX.Mathematics.Interop;
using Ellipse = SharpDX.Direct2D1.Ellipse;

namespace Geisha.Engine.Rendering.DirectX;

internal static class ConversionExtensions
{
    public static RawRectangleF ToRawRectangleF(this in AxisAlignedRectangle rectangle) =>
        new((float)rectangle.UpperLeft.X, (float)-rectangle.UpperLeft.Y,
            (float)rectangle.LowerRight.X, (float)-rectangle.LowerRight.Y);

    public static Ellipse ToDirectXEllipse(this Core.Math.Ellipse ellipse) =>
        new(new RawVector2((float)ellipse.Center.X, (float)-ellipse.Center.Y), (float)ellipse.RadiusX, (float)ellipse.RadiusY);

    public static RawColor4 ToRawColor4(this Color color) =>
        new((float)color.DoubleR, (float)color.DoubleG, (float)color.DoubleB, (float)color.DoubleA);

    public static Size2 ToSize2(this Size size) => new(size.Width, size.Height);
    public static Size ToSize(this Size2 size2) => new(size2.Width, size2.Height);

    public static SharpDX.Direct2D1.BitmapInterpolationMode ToDirectXBitmapInterpolationMode(this BitmapInterpolationMode bitmapInterpolationMode) =>
        bitmapInterpolationMode switch
        {
            BitmapInterpolationMode.NearestNeighbor => SharpDX.Direct2D1.BitmapInterpolationMode.NearestNeighbor,
            BitmapInterpolationMode.Linear => SharpDX.Direct2D1.BitmapInterpolationMode.Linear,
            _ => throw new System.ArgumentOutOfRangeException(nameof(bitmapInterpolationMode), bitmapInterpolationMode, null)
        };
}