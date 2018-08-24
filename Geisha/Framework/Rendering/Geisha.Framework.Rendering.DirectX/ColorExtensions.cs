using SharpDX.Mathematics.Interop;

namespace Geisha.Framework.Rendering.DirectX
{
    internal static class ColorExtensions
    {
        public static RawColor4 ToRawColor4(this Color color)
        {
            return new RawColor4((float) color.ScR, (float) color.ScG, (float) color.ScB, (float) color.ScA);
        }
    }
}