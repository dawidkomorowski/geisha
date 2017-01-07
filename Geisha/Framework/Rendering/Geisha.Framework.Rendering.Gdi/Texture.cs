using System.Drawing;
using System.IO;

namespace Geisha.Framework.Rendering.Gdi
{
    public class Texture : ITexture
    {
        private readonly Bitmap _bitmap;

        public Texture(Stream stream)
        {
            _bitmap = new Bitmap(stream);
        }

        public Bitmap Bitmap => _bitmap;
    }
}