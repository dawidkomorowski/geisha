using Geisha.Common.Geometry;

namespace Geisha.Framework.Rendering
{
    public interface IRenderer2D : IRenderer
    {
        void RenderSprite(Sprite sprite, Matrix3 transform);
        void RenderText(string text, int fontSize, Color color, Matrix3 transform);
    }
}