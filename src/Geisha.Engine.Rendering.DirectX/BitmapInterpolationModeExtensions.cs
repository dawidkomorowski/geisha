using Geisha.Engine.Rendering.Backend;

namespace Geisha.Engine.Rendering.DirectX;

internal static class BitmapInterpolationModeExtensions
{
    public static SharpDX.Direct2D1.BitmapInterpolationMode ToDirectXBitmapInterpolationMode(this BitmapInterpolationMode bitmapInterpolationMode)
    {
        return bitmapInterpolationMode switch
        {
            BitmapInterpolationMode.NearestNeighbor => SharpDX.Direct2D1.BitmapInterpolationMode.NearestNeighbor,
            BitmapInterpolationMode.Linear => SharpDX.Direct2D1.BitmapInterpolationMode.Linear,
            _ => throw new System.ArgumentOutOfRangeException(nameof(bitmapInterpolationMode), bitmapInterpolationMode, null)
        };
    }
}