using System;
using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Physics.Components
{
    /// <summary>
    ///     2D collider component in shape of a rectangle.
    /// </summary>
    public sealed class RectangleColliderComponent : Collider2DComponent
    {
        public static ComponentId Id { get; } = new ComponentId("Geisha.Engine.Physics.RectangleColliderComponent");

        public override ComponentId ComponentId => Id;

        /// <summary>
        ///     Dimension of rectangle. Rectangle has center at point (0,0) in local coordinate system.
        /// </summary>
        public Vector2 Dimension { get; set; }
    }

    internal sealed class RectangleColliderComponentFactory : IComponentFactory
    {
        public Type ComponentType { get; } = typeof(RectangleColliderComponent);
        public ComponentId ComponentId => RectangleColliderComponent.Id;
        public IComponent Create() => new RectangleColliderComponent();
    }
}