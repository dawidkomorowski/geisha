using System;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Systems;

namespace Geisha.Engine.Physics.Components;

/// <summary>
///     Base class for 2D colliders components.
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

    // TODO Update documentation on performance suggestions.
    // TODO Should it be IReadOnlyList<Contact2D> or List<Contact2D>?
    // TODO Can it be rephrased to be more clear?
    /// <summary>
    ///     Gets all contacts present for this collider. Contact is present when two colliders are in contact.
    /// </summary>
    /// <returns>Collection of all contacts present for this collider.</returns>
    /// <remarks>TBD</remarks>
    public Contact2D[] GetContacts() => PhysicsBodyProxy?.GetContacts() ?? Array.Empty<Contact2D>();
}