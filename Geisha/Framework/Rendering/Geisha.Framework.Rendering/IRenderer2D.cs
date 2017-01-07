using System;
using Geisha.Common.Geometry;

namespace Geisha.Framework.Rendering
{
    public interface IRenderer2D : IRenderer
    {
        [Obsolete]
        void Render(ITexture texture, int x, int y);

        void Render(ITexture texture, Vector2 position);
    }
}