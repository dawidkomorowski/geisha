using System;
using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Physics.Components;

/// <summary>
///     Base class for 2D colliders components.
/// </summary>
public abstract class Collider2DComponent : Component
{
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
    ///     Indicates whether this collider is in contact with the other one.
    /// </summary>
    public bool IsColliding => Contacts.Count > 0;

    /// <summary>
    ///     Collection of all contacts present for this collider. Contact is present when two colliders are in contact.
    /// </summary>
    public IReadOnlyList<Contact2D> Contacts => _contacts;

    internal void AddContact(Contact2D contact)
    {
        _contacts.Add(contact);
    }

    internal void ClearContacts()
    {
        _contacts.Clear();
    }
}