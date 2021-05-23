using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Physics.Components
{
    /// <summary>
    ///     2D collider component in shape of a circle.
    /// </summary>
    [ComponentId("Geisha.Engine.Physics.CircleColliderComponent")]
    public sealed class CircleColliderComponent : Collider2DComponent
    {
        /// <summary>
        ///     Radius of circle.
        /// </summary>
        public double Radius { get; set; }
    }

    internal sealed class CircleColliderComponentFactory : ComponentFactory<CircleColliderComponent>
    {
        protected override CircleColliderComponent CreateComponent() => new CircleColliderComponent();
    }
}