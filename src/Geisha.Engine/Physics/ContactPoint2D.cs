using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics;

/// <summary>
///     Represents a contact point between two colliders, including its position in both world and local coordinates.
/// </summary>
/// <remarks>
///     <see cref="ContactPoint2D" /> represents a point where two colliders intersect during a collision. All contact
///     points for a given collider pair are stored in the <see cref="Contact2D.ContactPoints" /> property.
/// </remarks>
public readonly record struct ContactPoint2D
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ContactPoint2D" /> struct.
    /// </summary>
    /// <param name="worldPosition">The position of the contact point in world coordinates.</param>
    /// <param name="thisLocalPosition">
    ///     The position of the contact point in local coordinates of <see cref="Contact2D.ThisCollider" />.
    /// </param>
    /// <param name="otherLocalPosition">
    ///     The position of the contact point in local coordinates of <see cref="Contact2D.OtherCollider" />.
    /// </param>
    public ContactPoint2D(in Vector2 worldPosition, in Vector2 thisLocalPosition, in Vector2 otherLocalPosition)
    {
        WorldPosition = worldPosition;
        ThisLocalPosition = thisLocalPosition;
        OtherLocalPosition = otherLocalPosition;
    }

    /// <summary>
    ///     Gets the position of the contact point in world coordinates.
    /// </summary>
    public Vector2 WorldPosition { get; }

    /// <summary>
    ///     Gets the position of the contact point in local coordinates of <see cref="Contact2D.ThisCollider" />.
    /// </summary>
    /// <remarks>
    ///     The local position takes into account the rotation of the rigid body. For example, if the body is rotated by 90
    ///     degrees counterclockwise, the local position of a contact point at the bottom of the body would correspond to the
    ///     right side of the body in world coordinates.
    /// </remarks>
    public Vector2 ThisLocalPosition { get; }

    /// <summary>
    ///     Gets the position of the contact point in local coordinates of <see cref="Contact2D.OtherCollider" />.
    /// </summary>
    /// <remarks>
    ///     The local position takes into account the rotation of the rigid body. For example, if the body is rotated by 90
    ///     degrees counterclockwise, the local position of a contact point at the bottom of the body would correspond to the
    ///     right side of the body in world coordinates.
    /// </remarks>
    public Vector2 OtherLocalPosition { get; }
}