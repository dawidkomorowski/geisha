using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Physics.Components;

/// <summary>
///     Base class for 2D colliders components.
/// </summary>
public abstract class Collider2DComponent : Component
{
    private readonly HashSet<Entity> _collidingEntities = new();
    private readonly List<Contact> _contacts = new();

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

    public IReadOnlyList<Contact> Contacts => _contacts;

    internal void ClearCollidingEntities()
    {
        _collidingEntities.Clear();
    }

    internal void AddCollidingEntity(Entity entity)
    {
        _collidingEntities.Add(entity);
    }

    internal void AddContact(Contact contact)
    {
        _contacts.Add(contact);
    }
}

public readonly struct Contact
{
    public Collider2DComponent ThisCollider { get; }
    public Collider2DComponent OtherCollider { get; }
    public Vector2 CollisionNormal { get; } // TODO Is it from This to Other or from Other to This?
    public double SeparationDepth { get; }
}

// TODO How useful would it be?
// TODO How to solve 'readonly' structs embedding non-readonly fixed list?
public struct FixedList2<T>
{
    private T _item0;
    private T _item1;

    public int Count { get; private set; }
}