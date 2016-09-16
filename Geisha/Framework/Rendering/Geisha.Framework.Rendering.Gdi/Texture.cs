using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geisha.Framework.Rendering.Gdi
{
    public class Texture : ITexture
    {
        private readonly Bitmap _bitmap;

        public Texture(Stream stream)
        {
            _bitmap = new Bitmap(stream);
        }
    }
}
