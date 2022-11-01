namespace Geisha.Engine.Physics
{
    /// <summary>
    ///     Configuration of engine physics subsystem.
    /// </summary>
    public sealed record PhysicsConfiguration
    {
        /// <summary>
        ///     If true, collision geometry is rendered on top of regular graphics to help with debugging.
        /// </summary>
        public bool RenderCollisionGeometry { get; init; } = false;
    }
}