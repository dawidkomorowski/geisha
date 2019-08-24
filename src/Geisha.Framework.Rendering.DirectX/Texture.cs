using Geisha.Common.Math;
using Geisha.Engine.Rendering;
using SharpDX.Direct2D1;

namespace Geisha.Framework.Rendering.DirectX
{
    public sealed class Texture : ITexture
    {
        internal readonly Bitmap D2D1Bitmap;

        public Texture(Bitmap d2D1Bitmap)
        {
            D2D1Bitmap = d2D1Bitmap;
        }

        public Vector2 Dimension => new Vector2(D2D1Bitmap.PixelSize.Width, D2D1Bitmap.PixelSize.Height);

        public void Dispose()
        {
            D2D1Bitmap.Dispose();
        }
    }
}