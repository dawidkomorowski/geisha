using Geisha.Common.Geometry;

namespace Geisha.Framework.Rendering
{
    public interface IRenderer2D : IRenderer
    {
        void Render(Sprite sprite, Matrix3 transform);
    }
}