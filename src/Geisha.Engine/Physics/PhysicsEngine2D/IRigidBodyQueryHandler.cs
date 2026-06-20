namespace Geisha.Engine.Physics.PhysicsEngine2D;

/// <summary>
///     Defines callback used by rigid body scene queries to process matching bodies.
/// </summary>
/// <remarks>
///     Query methods invoke <see cref="Handle" /> for each body that matches query predicate.
///     The return value controls whether query iteration should continue.
/// </remarks>
internal interface IRigidBodyQueryHandler
{
    /// <summary>
    ///     Handles a rigid body returned by a query.
    /// </summary>
    /// <param name="body">Rigid body that matched query predicate.</param>
    /// <returns>
    ///     <see langword="true" /> to continue query iteration and process next matching body;
    ///     <see langword="false" /> to stop query iteration immediately.
    /// </returns>
    bool Handle(RigidBody2D body);
}

internal interface IRigidBodyIdQueryHandler
{
    bool Handle(RigidBodyId id);
}