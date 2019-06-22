using System;
using Geisha.Framework.Rendering;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Ellipse renderer component enables entity with ellipse rendering functionality.
    /// </summary>
    public sealed class EllipseRendererComponent : Renderer2DComponent
    {
        /// <summary>
        ///     Radius of the circle.
        /// </summary>
        /// <remarks>
        ///     If assigned changes ellipse to circle by modifying <see cref="RadiusX" /> and <see cref="RadiusY" />. When
        ///     accessed returns circle radius if ellipse is circle; otherwise throws <see cref="EllipseIsNotCircleException" />.
        /// </remarks>
        /// <exception cref="EllipseIsNotCircleException">Thrown when ellipse does not represent a circle.</exception>
        public double Radius
        {
            get => RadiusX == RadiusY ? RadiusX : throw new EllipseIsNotCircleException(RadiusX, RadiusY);
            set
            {
                RadiusX = value;
                RadiusY = value;
            }
        }

        /// <summary>
        ///     X radius of the ellipse.
        /// </summary>
        public double RadiusX { get; set; }

        /// <summary>
        ///     Y radius of the ellipse.
        /// </summary>
        public double RadiusY { get; set; }

        /// <summary>
        ///     Color of the ellipse.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        ///     Specifies whether to fill interior of ellipse or draw only border. If true interior is filled with color.
        /// </summary>
        public bool FillInterior { get; set; }
    }

    /// <summary>
    ///     The exception that is thrown when accessing <see cref="EllipseRendererComponent.Radius" /> when ellipse does not
    ///     represent a circle.
    /// </summary>
    public sealed class EllipseIsNotCircleException : Exception
    {
        public EllipseIsNotCircleException(double radiusX, double radiusY)
            : base($"Ellipse is not a circle. Circle radius cannot be calculated. RadiusX = {radiusX}, RadiusY = {radiusY}.")
        {
            RadiusX = radiusX;
            RadiusY = radiusY;
        }

        /// <summary>
        ///     X radius of the ellipse.
        /// </summary>
        public double RadiusX { get; }

        /// <summary>
        ///     Y radius of the ellipse.
        /// </summary>
        public double RadiusY { get; }
    }
}