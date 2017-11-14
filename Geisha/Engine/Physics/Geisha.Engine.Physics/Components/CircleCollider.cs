namespace Geisha.Engine.Physics.Components
{
    /// <summary>
    ///     2D collider component in shape of a circle.
    /// </summary>
    public sealed class CircleCollider : Collider2D
    {
        /// <summary>
        ///     Radius of circle.
        /// </summary>
        public double Radius { get; set; }
    }
}