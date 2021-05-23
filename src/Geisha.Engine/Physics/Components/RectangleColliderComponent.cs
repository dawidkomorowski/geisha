using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Physics.Components
{
    /// <summary>
    ///     2D collider component in shape of a rectangle.
    /// </summary>
    [ComponentId("Geisha.Engine.Physics.RectangleColliderComponent")]
    public sealed class RectangleColliderComponent : Collider2DComponent
    {
        /// <summary>
        ///     Dimension of rectangle. Rectangle has center at point (0,0) in local coordinate system.
        /// </summary>
        public Vector2 Dimension { get; set; }
    }

    internal sealed class RectangleColliderComponentFactory : ComponentFactory<RectangleColliderComponent>
    {
        protected override RectangleColliderComponent CreateComponent() => new RectangleColliderComponent();
    }
}