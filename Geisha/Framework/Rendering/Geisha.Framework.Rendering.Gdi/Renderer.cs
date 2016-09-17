using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geisha.Framework.Rendering.Gdi
{
    public class Renderer : IRenderer
    {
        public ITexture CreateTexture(Stream stream)
        {
            return new Texture(stream);
        }
    }
}
