using System.Drawing;
using System.IO;
using Geisha.Common.Math;

namespace Geisha.Framework.Rendering.Gdi
{
    public class Texture : ITexture
    {
        public Vector2 Dimension => new Vector2(Bitmap.Width, Bitmap.Height);
        public Bitmap Bitmap { get; }


        public Texture(Stream stream)
        {
            using (var image = Image.FromStream(stream))
            {
                Bitmap = new Bitmap(image);
            }
        }

        public void Dispose()
        {
            Bitmap.Dispose();
        }
    }
}