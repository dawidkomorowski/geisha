using System;
using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Physics.Components;

/// <summary>
///     Base class for 2D colliders components.
/// </summary>
public abstract class Collider2DComponent : Component
{
    private readonly HashSet<Entity> _collidingEntities = new();
    private readonly List<Contact2D> _contacts = new();

    /// <summary>
    ///     Initializes new instance of <see cref="Collider2DComponent" /> class which is attached to specified entity.
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
    ///     Indicates whether this collider is colliding with some other one.
    /// </summary>
    public bool IsColliding => CollidingEntities.Count > 0;

    /// <summary>
    ///     Collection of all entities colliding with this collider.
    /// </summary>
    public IReadOnlyCollection<Entity> CollidingEntities => _collidingEntities;

    public IReadOnlyList<Contact2D> Contacts => _contacts;

    internal void ClearCollidingEntities()
    {
        _collidingEntities.Clear();
    }

    internal void AddCollidingEntity(Entity entity)
    {
        _collidingEntities.Add(entity);
    }

    internal void AddContact(Contact2D contact)
    {
        _contacts.Add(contact);
    }
}