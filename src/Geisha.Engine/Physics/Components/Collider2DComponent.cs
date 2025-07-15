using System;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Systems;

namespace Geisha.Engine.Physics.Components;

/// <summary>
///     Base class for 2D collider components.
/// </summary>
public abstract class Collider2DComponent : Component
{
    internal PhysicsBodyProxy? PhysicsBodyProxy { get; set; }

    /// <summary>
    ///     Initializes a new instance of <see cref="Collider2DComponent" /> class which is attached to specified entity.
    /// </summary>
    /// <param name="entity">Entity to which new component is attached.</param>
    protected Collider2DComponent(Entity entity) : base(entity)
    {
        foreach (var component in entity.Components)
        {
            if (component is Collider2DComponent)
            {
                throw new ArgumentException($"{nameof(Collider2DComponent)} is already added to entity.");
            }
        }
    }

    /// <summary>
    ///     Indicates whether this collider is in contact with the other one.
    /// </summary>
    public bool IsColliding => PhysicsBodyProxy?.IsColliding ?? false;

    /// <summary>
    ///     Retrieves all contacts currently involving this collider. A contact exists when two colliders are overlapping.
    /// </summary>
    /// <returns>An array containing all current contacts involving this collider.</returns>
    /// <remarks>
    ///     <para>
    ///         <see cref="GetContacts" /> returns all current contacts involving this collider. If you are only interested in
    ///         whether the collider is colliding with any other collider, use <see cref="IsColliding" /> instead for better
    ///         performance.
    ///     </para>
    ///     <para>
    ///         <see cref="GetContacts" /> allocates an array every time it is called and converts internal contacts to
    ///         <see cref="Contact2D" />. It is recommended to avoid calling this method frequently. Instead, consider caching
    ///         the result and reusing it when needed.
    ///     </para>
    /// </remarks>
    public Contact2D[] GetContacts() => PhysicsBodyProxy?.GetContacts() ?? Array.Empty<Contact2D>();
}