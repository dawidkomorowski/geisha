using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geisha.Framework.Rendering.Gdi
{
    public abstract class Renderer : IRenderer
    {
        private readonly RenderingContext _renderingContext;

        protected Renderer(RenderingContext renderingContext)
        {
            _renderingContext = renderingContext;
        }

        public ITexture CreateTexture(Stream stream)
        {
            return new Texture(stream);
        }

        public void Clear()
        {
            using (Graphics graphics = Graphics.FromImage(_renderingContext.Bitmap))
            {
                graphics.Clear(Color.White);
            }
        }
    }
}
