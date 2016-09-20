using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geisha.Framework.Rendering.Gdi
{
    public class Renderer2D : Renderer, IRenderer2D
    {
        private readonly RenderingContext _renderingContext;

        public Renderer2D(RenderingContext renderingContext) : base(renderingContext)
        {
            _renderingContext = renderingContext;
        }

        public void Render(ITexture texture, int x, int y)
        {
            Render((Texture)texture, x, y);
        }

        private void Render(Texture texture, int x, int y)
        {
            using (var graphics = Graphics.FromImage(_renderingContext.Bitmap))
            {
                graphics.DrawImage(texture.Bitmap, x, y);
            }
        }
    }
}
