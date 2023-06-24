using Geisha.Engine.Core.Math;
using SharpDX.Direct2D1;

namespace Geisha.Engine.Rendering.DirectX
{
    internal sealed class Texture : ITexture
    {
        internal readonly Bitmap D2D1Bitmap;

        public Texture(Bitmap d2D1Bitmap)
        {
            D2D1Bitmap = d2D1Bitmap;
        }

        public Vector2 Dimensions => new(D2D1Bitmap.PixelSize.Width, D2D1Bitmap.PixelSize.Height);

        public void Dispose()
        {
            D2D1Bitmap.Dispose();
        }
    }
}