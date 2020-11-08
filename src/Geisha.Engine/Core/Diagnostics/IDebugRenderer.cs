using Geisha.Common.Math;

namespace Geisha.Engine.Core.Diagnostics
{
    // TODO Add documentation.
    public interface IDebugRenderer
    {
        void DrawCircle(Circle circle, Color color);
        void DrawRectangle(Rectangle rectangle, Color color, Matrix3x3 transform);
    }
}