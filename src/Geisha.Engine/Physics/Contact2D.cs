using Geisha.Engine.Core.Collections;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;

namespace Geisha.Engine.Physics;

/// <summary>
///     Represents a contact between two 2D colliders in the physics simulation. Contains information about the colliders
///     involved, the collision normal, penetration depth, and contact points.
/// </summary>
/// <remarks>
///     <see cref="Contact2D" /> represents a contact from the perspective of <see cref="ThisCollider" />. Analogously, the
///     corresponding contact can be obtained from the perspective of <see cref="OtherCollider" /> by retrieving the
///     contacts from the other collider.
/// </remarks>
public class Contact2D
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Contact2D" /> class.
    /// </summary>
    /// <param name="thisCollider">The collider involved in the contact.</param>
    /// <param name="otherCollider">The other collider involved in the contact.</param>
    /// <param name="collisionNormal">
    ///     The normal vector of the collision pointing from <paramref name="otherCollider" /> to
    ///     <paramref name="thisCollider" />.
    /// </param>
    /// <param name="penetrationDepth">The depth of penetration between the colliders.</param>
    /// <param name="contactPoints">The contact points where the colliders intersect.</param>
    public Contact2D(
        Collider2DComponent thisCollider,
        Collider2DComponent otherCollider,
        in Vector2 collisionNormal,
        double penetrationDepth,
        in ReadOnlyFixedList2<ContactPoint2D> contactPoints
    )
    {
        ThisCollider = thisCollider;
        OtherCollider = otherCollider;
        CollisionNormal = collisionNormal;
        PenetrationDepth = penetrationDepth;
        ContactPoints = contactPoints;
    }

    /// <summary>
    ///     Gets the collider involved in the contact.
    /// </summary>
    public Collider2DComponent ThisCollider { get; }

    /// <summary>
    ///     Gets the other collider involved in the contact.
    /// </summary>
    public Collider2DComponent OtherCollider { get; }

    /// <summary>
    ///     Gets the normal vector of the collision. Normal vector is a unit vector pointing from <see cref="OtherCollider" />
    ///     to <see cref="ThisCollider" />.
    /// </summary>
    public Vector2 CollisionNormal { get; }

    /// <summary>
    ///     Gets the depth of penetration between the colliders.
    /// </summary>
    /// <remarks>
    ///     The penetration depth indicates how far the colliders are overlapping. The greater the value, the deeper the
    ///     penetration.
    /// </remarks>
    public double PenetrationDepth { get; }

    /// <summary>
    ///     Gets the contact points where the colliders intersect.
    /// </summary>
    /// <remarks>
    ///     Actual number of contact points may vary depending on the collision shape and the nature of the contact.
    /// </remarks>
    public ReadOnlyFixedList2<ContactPoint2D> ContactPoints { get; }
}