using System.Drawing;

namespace Geisha.Framework.Rendering.Gdi
{
    public class RenderingContext : IRenderingContext
    {
        private readonly Bitmap _bitmap;

        public RenderingContext(Bitmap bitmap)
        {
            _bitmap = bitmap;
        }

        public Bitmap Bitmap => _bitmap;
        public int RenderTargetWidth => _bitmap.Width;
        public int RenderTargetHeight => _bitmap.Height;
    }
}