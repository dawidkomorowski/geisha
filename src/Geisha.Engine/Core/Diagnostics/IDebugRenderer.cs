using Geisha.Common.Math;

namespace Geisha.Engine.Core.Diagnostics
{
    public interface IDebugRenderer
    {
        void DrawCircle(Circle circle, Color color);
    }
}