using System.IO;

namespace Geisha.Framework.Rendering
{
    public interface IRenderer
    {
        IRenderingContext RenderingContext { get; }
        ITexture CreateTexture(Stream stream);
        void Clear();
    }
}