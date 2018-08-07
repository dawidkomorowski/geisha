using Geisha.Common.Math;
using SharpDX.Direct2D1;

namespace Geisha.Framework.Rendering.DirectX
{
    public sealed class Texture : ITexture
    {
        private readonly Bitmap _d2D1Bitmap;

        public Texture(Bitmap d2D1Bitmap)
        {
            _d2D1Bitmap = d2D1Bitmap;
        }

        public Vector2 Dimension => new Vector2(_d2D1Bitmap.PixelSize.Width, _d2D1Bitmap.PixelSize.Height);

        public void Dispose()
        {
            _d2D1Bitmap.Dispose();
        }
    }
}