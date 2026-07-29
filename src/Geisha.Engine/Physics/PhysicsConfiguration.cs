using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics;

/// <summary>
///     Represents the configuration settings for the physics subsystem of the engine, allowing fine-tuning of simulation
///     precision, stability, and debugging options.
/// </summary>
public sealed record PhysicsConfiguration
{
    /// <summary>
    ///     Defines how many substeps are performed during physics simulation per each game loop fixed update.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Increasing number of substeps will improve precision and stability of physics simulation, but it will be at
    ///         cost of performance.
    ///     </para>
    ///     <para>
    ///         In general, increasing the number of substeps will behave in a similar way as increasing the number of fixed
    ///         updates per second. However, substeps only affect physics simulation therefore it does not have an overhead of
    ///         increased frequency of synchronization of Physics Engine state with Scene state.
    ///     </para>
    /// </remarks>
    public int Substeps { get; init; } = 1;

    /// <summary>
    ///     Defines how many iterations of velocity constraint solver are performed during physics simulation per each physics
    ///     step (or substep).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Increasing number of velocity iterations will improve precision and stability of physics simulation, but it
    ///         will be at cost of performance.
    ///     </para>
    ///     <para>
    ///         Number of velocity iterations defines how many iterations a velocity constraint solver will perform. Each
    ///         iteration all velocity constraints are solved one by one, which means that the more iterations are performed,
    ///         the more accurate is the final velocity of bodies in the physics simulation.
    ///     </para>
    /// </remarks>
    public int VelocityIterations { get; init; } = 4;

    /// <summary>
    ///     Defines how many iterations of position constraint solver are performed during physics simulation per each physics
    ///     step (or substep).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Increasing number of position iterations will improve precision and stability of physics simulation, but it
    ///         will be at cost of performance.
    ///     </para>
    ///     <para>
    ///         Number of position iterations defines how many iterations a position constraint solver will perform. Each
    ///         iteration all position constraints are solved one by one, which means that the more iterations are performed,
    ///         the more accurate is the final position of bodies in the physics simulation.
    ///     </para>
    /// </remarks>
    public int PositionIterations { get; init; } = 4;

    /// <summary>
    ///     Defines a tolerance for penetration resolution in physics simulation.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         When the penetration between two colliding bodies is less than tolerance, the physics engine will constrain
    ///         the bodies to prevent further penetration. However, the engine will not resolve penetration by fixing the
    ///         bodies' positions.
    ///     </para>
    ///     <para>
    ///         When the penetration is greater than this tolerance, the physics engine will resolve the penetration by
    ///         adjusting the positions of the bodies.
    ///     </para>
    ///     <para>This parameter is useful to prevent unstable contact generation that can lead to jittering of bodies.</para>
    /// </remarks>
    public double PenetrationTolerance { get; init; } = 0.01;

    /// <summary>
    ///     Specifies the tile size used by the physics engine. The physics engine allows defining tile-based collision
    ///     geometry and this property represents the size of a single rectangular tile. Tile size is defined in meters.
    /// </summary>
    /// <seealso cref="Components.TileColliderComponent" />
    public SizeD TileSize { get; init; } = new(1.0, 1.0);

    /// <summary>
    ///     Specifies the size of a single cell of the uniform grid used by the broad phase of the physics engine. Cell size
    ///     is defined in meters.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Before checking pairs of bodies for actual collisions, the physics engine uses a uniform grid to narrow down
    ///         the set of candidate pairs to those that are close to each other. Each body is indexed in every cell its
    ///         bounding box overlaps. This property controls the size of those cells and therefore how bodies are
    ///         distributed among them. It affects performance of collision detection and physics scene queries, but not the
    ///         outcome of the simulation.
    ///     </para>
    ///     <para>
    ///         Cell size that is large relative to the bodies in the scene results in fewer occupied cells, but more bodies
    ///         per cell, so more candidate pairs reach the more expensive exact collision checks. Cell size that is small
    ///         relative to the bodies results in each body being indexed in more cells, which increases the cost of keeping
    ///         the grid up to date as bodies move. The most suitable value depends on the size and spatial distribution of
    ///         bodies in a particular game, so it is best established by measurement.
    ///     </para>
    ///     <para>
    ///         Both dimensions must be greater than zero. The physics system throws
    ///         <see cref="System.ArgumentException" /> during initialization when this requirement is not met.
    ///     </para>
    ///     <para>
    ///         This value is applied when the physics system is created and it cannot be changed afterwards.
    ///     </para>
    /// </remarks>
    public SizeD BroadPhaseGridCellSize { get; init; } = new(256, 256);

    /// <summary>
    ///     Indicates whether physics debug rendering is enabled.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         When enabled, the physics system renders debug visualizations (for example, collision geometry) on top of the
    ///         standard graphics output to assist with debugging.
    ///     </para>
    ///     <para>
    ///         This configuration sets the initial state and can be toggled at runtime via the physics system API.
    ///     </para>
    /// </remarks>
    /// <seealso cref="Systems.IPhysicsSystem" />
    public bool EnableDebugRendering { get; init; } = false;
}