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

public class Contact2D
{
    public Collider2DComponent ThisCollider { get; }
    public Collider2DComponent OtherCollider { get; }
    public Vector2 CollisionNormal { get; } // TODO Is it from This to Other or from Other to This?
    public double SeparationDepth { get; } // TODO SeparationDepth or Separation or something else?
    public ReadOnlyFixedList2<ContactPoint2D> ContactPoints { get; }
}

// TODO How useful would it be?
// TODO How to solve 'readonly' structs embedding non-readonly fixed list?
public struct FixedList2<T>
{
    private T? _item0;
    private T? _item1;

    public FixedList2()
    {
        _item0 = default;
        _item1 = default;
        Count = 0;
    }

    public static int Capacity => 2;
    public int Count { get; private set; }

    public T this[int index]
    {
        get
        {
            if (index >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            return index switch
            {
                0 => _item0!,
                1 => _item1!,
                _ => throw new IndexOutOfRangeException()
            };
        }
        set
        {
            if (index >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            switch (index)
            {
                case 0:
                    _item0 = value;
                    break;
                case 1:
                    _item1 = value;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
    }

    public void Add(T? item)
    {
        if (Count == Capacity)
        {
            throw new InvalidOperationException("TODO");
        }

        this[Count++] = item!;
    }

    public ReadOnlyFixedList2<T> ToReadOnly() => Count switch
    {
        0 => new ReadOnlyFixedList2<T>(),
        1 => new ReadOnlyFixedList2<T>(_item0),
        2 => new ReadOnlyFixedList2<T>(_item0, _item1),
        _ => throw new IndexOutOfRangeException()
    };
}

public readonly struct ReadOnlyFixedList2<T>
{
    private readonly T? _item0;
    private readonly T? _item1;

    public ReadOnlyFixedList2()
    {
        _item0 = default;
        _item1 = default;
        Count = 0;
    }

    public ReadOnlyFixedList2(T? item0)
    {
        _item0 = item0;
        _item1 = default;
        Count = 1;
    }

    public ReadOnlyFixedList2(T? item0, T? item1)
    {
        _item0 = item0;
        _item1 = item1;
        Count = 2;
    }

    public int Count { get; }

    public T this[int index]
    {
        get
        {
            if (index >= Count)
            {
                throw new IndexOutOfRangeException();
            }

            return index switch
            {
                0 => _item0!,
                1 => _item1!,
                _ => throw new IndexOutOfRangeException()
            };
        }
    }
}

public readonly struct ContactPoint2D
{
    public ContactPoint2D(in Vector2 worldPosition, in Vector2 thisLocalPosition, in Vector2 otherLocalPosition)
    {
        WorldPosition = worldPosition;
        ThisLocalPosition = thisLocalPosition;
        OtherLocalPosition = otherLocalPosition;
    }

    public Vector2 WorldPosition { get; }
    public Vector2 ThisLocalPosition { get; }
    public Vector2 OtherLocalPosition { get; }
}