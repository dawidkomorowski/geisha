using System;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Physics.Components
{
    /// <summary>
    ///     2D collider component in shape of a circle.
    /// </summary>
    public sealed class CircleColliderComponent : Collider2DComponent
    {
        /// <summary>
        ///     Radius of circle.
        /// </summary>
        public double Radius { get; set; }
    }

    internal sealed class CircleColliderComponentFactory : IComponentFactory
    {
        public Type ComponentType { get; } = typeof(CircleColliderComponent);
        public ComponentId ComponentId { get; } = new ComponentId("Geisha.Engine.Physics.CircleColliderComponent");
        public IComponent Create() => new CircleColliderComponent();
    }
}