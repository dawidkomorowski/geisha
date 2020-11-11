using Geisha.Common.Math;

namespace Geisha.Engine.Core.Diagnostics
{
    /// <summary>
    ///     Provides API for rendering debugging information on the screen.
    /// </summary>
    public interface IDebugRenderer
    {
        /// <summary>
        ///     Draws <paramref name="circle" /> with specified <paramref name="color" />.
        /// </summary>
        /// <param name="circle">Circle geometry to draw.</param>
        /// <param name="color">Color of circle.</param>
        void DrawCircle(Circle circle, Color color);

        /// <summary>
        ///     Draws <paramref name="rectangle" /> with specified <paramref name="color" /> and applying specified
        ///     <paramref name="transform" />.
        /// </summary>
        /// <param name="rectangle">Rectangle geometry to draw.</param>
        /// <param name="color">Color of rectangle.</param>
        /// <param name="transform">Transform to be applied fo drawing the rectangle.</param>
        void DrawRectangle(Rectangle rectangle, Color color, Matrix3x3 transform);
    }
}