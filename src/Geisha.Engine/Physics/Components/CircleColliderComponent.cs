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
}