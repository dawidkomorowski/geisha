namespace Geisha.Engine.Physics.Systems;

/// <summary>
///     Provides access to runtime settings and controls of the physics system.
/// </summary>
/// <seealso cref="Geisha.Engine.Physics.PhysicsConfiguration" />
public interface IPhysicsSystem
{
    /// <summary>
    ///     Gets or sets a value indicating whether physics debug rendering is enabled.
    /// </summary>
    /// <remarks>
    ///     When enabled, the physics system renders debug visualizations (for example, collision geometry) on top of the
    ///     standard graphics output to assist with debugging.
    /// </remarks>
    bool EnableDebugRendering { get; set; }
}