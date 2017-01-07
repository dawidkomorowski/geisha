using System.IO;

namespace Geisha.Framework.Rendering
{
    public interface IRenderer
    {
        ITexture CreateTexture(Stream stream);
        void Clear();
    }
}