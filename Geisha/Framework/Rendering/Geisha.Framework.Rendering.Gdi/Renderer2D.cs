using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geisha.Framework.Rendering.Gdi
{
    public class Renderer2D : IRenderer2D<Texture>
    {
        public void Render(Texture texture)
        {
            texture.Bitmap.GetPixel(0,0);
        }

        public void Render(ITexture texture)
        {
            Render((Texture)texture);
        }
    }
}
