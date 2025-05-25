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
        ///         cost of decreased performance.
        ///     </para>
        ///     <para>
        ///         In general, increasing the number of substeps will behave in a similar way as increasing the number of fixed
        ///         updates per second. However, substeps only affect physics simulation therefore it does not have an overhead of
        ///         increased frequency of synchronization of Physics Engine state with Scene state.
        ///     </para>
        /// </remarks>
        public int Substeps { get; init; } = 1;

        /// <summary>
        ///     If true, collision geometry is rendered on top of regular graphics to help with debugging.
        /// </summary>
        public bool RenderCollisionGeometry { get; init; } = false;
    }
}