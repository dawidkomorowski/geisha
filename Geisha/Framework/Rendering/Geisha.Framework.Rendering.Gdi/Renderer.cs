using System.Drawing;
using System.IO;

namespace Geisha.Framework.Rendering.Gdi
{
    public abstract class Renderer : IRenderer
    {
        protected readonly RenderingContext _renderingContext;

        protected Renderer(IRenderingContextFactory renderingContextFactory)
        {
            _renderingContext = (RenderingContext) renderingContextFactory.Create();
        }

        public ITexture CreateTexture(Stream stream)
        {
            return new Texture(stream);
        }

        public void Clear()
        {
            using (var graphics = Graphics.FromImage(_renderingContext.Bitmap))
            {
                graphics.Clear(Color.White);
            }
        }
    }
}