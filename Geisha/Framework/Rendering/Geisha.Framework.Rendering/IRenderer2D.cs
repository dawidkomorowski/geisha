using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geisha.Framework.Rendering
{
    public interface IRenderer2D
    {
        void Render(ITexture texture);
    }

    public interface IRenderer2D<TTexture> : IRenderer2D where TTexture : ITexture
    {
        void Render(TTexture texture);
    }
}
