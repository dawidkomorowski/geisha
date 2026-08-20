using Geisha.Engine.Core.Math;
using SharpDX;
using SharpDX.Mathematics.Interop;

namespace Geisha.Engine.Rendering.DirectX;

internal static class ColorExtensions
{
    public static RawColor4 ToRawColor4(this Color color)
    {
        return new RawColor4((float)color.DoubleR, (float)color.DoubleG, (float)color.DoubleB, (float)color.DoubleA);
    }
}

internal static class ConversionExtensions
{
    public static Size2 ToSize2(this Size size) => new(size.Width, size.Height);
    public static Size ToSize(this Size2 size2) => new(size2.Width, size2.Height);
}