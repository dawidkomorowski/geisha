using System;
using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Rendering.Components
{
    /// <summary>
    ///     Rectangle renderer component enables entity with rectangle rendering functionality.
    /// </summary>
    public sealed class RectangleRendererComponent : Renderer2DComponent
    {
        /// <summary>
        ///     Dimension of rectangle. Rectangle has center at point (0,0) in local coordinate system.
        /// </summary>
        public Vector2 Dimension { get; set; }

        /// <summary>
        ///     Color of the rectangle.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        ///     Specifies whether to fill interior of rectangle or draw only border. If true interior is filled with color.
        /// </summary>
        public bool FillInterior { get; set; }
    }

    internal sealed class RectangleRendererComponentFactory : IComponentFactory
    {
        public Type ComponentType { get; } = typeof(RectangleRendererComponent);
        public ComponentId ComponentId { get; } = new ComponentId("Geisha.Engine.Rendering.RectangleRendererComponent");
        public IComponent Create() => new RectangleRendererComponent();
    }
}