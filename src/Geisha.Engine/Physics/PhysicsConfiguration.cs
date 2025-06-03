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
    ///     geometry and this property represents the size of a single rectangular tile.
    /// </summary>
    public SizeD TileSize { get; init; } = new(1.0, 1.0);

    /// <summary>
    ///     Indicates whether collision geometry should be visually rendered over the standard graphics output to assist with
    ///     debugging physics interactions.
    /// </summary>
    public bool RenderCollisionGeometry { get; init; } = false;
}