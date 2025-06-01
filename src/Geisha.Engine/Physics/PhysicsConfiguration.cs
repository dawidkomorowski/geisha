namespace Geisha.Engine.Physics
{
    /// <summary>
    ///     Configuration of engine physics subsystem.
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

        // TODO Add documentation.
        public double PenetrationTolerance { get; init; } = 0.01;

        /// <summary>
        ///     If true, collision geometry is rendered on top of regular graphics to help with debugging.
        /// </summary>
        public bool RenderCollisionGeometry { get; init; } = false;
    }
}