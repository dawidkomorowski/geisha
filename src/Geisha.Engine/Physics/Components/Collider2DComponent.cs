using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Physics.Systems;

namespace Geisha.Engine.Physics.Components;

/// <summary>
///     Serves as the abstract base class for all 2D collider components.
/// </summary>
/// <remarks>
///     <para>
///         A collider component defines the shape and physical boundaries of an entity for the purposes of collision
///         detection and response in the physics simulation. Only one <see cref="Collider2DComponent" /> can be attached
///         to an entity.
///     </para>
///     <para>
///         Derived classes implement specific collider shapes, such as rectangles or circles, and provide the necessary
///         functionality to interact with the physics system.
///     </para>
/// </remarks>
public abstract class Collider2DComponent : Component
{
    internal PhysicsBodyProxy? PhysicsBodyProxy { get; set; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Collider2DComponent" /> class attached to the specified entity.
    /// </summary>
    /// <param name="entity">The entity to which the new collider component is attached.</param>
    /// <exception cref="ArgumentException">
    ///     Thrown if the entity already has a <see cref="Collider2DComponent" /> attached.
    /// </exception>
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
    ///     Gets or sets a value indicating whether this collider participates in physics simulation.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         When set to <see langword="true"/>, the collider is active and participates in collision detection and
    ///         contact generation. When set to <see langword="false"/>, the collider is ignored by the physics system and
    ///         does not produce contacts.
    ///     </para>
    ///     <para>
    ///         The default value is <see langword="true"/>.
    ///     </para>
    /// </remarks>
    public bool Enabled { get; set; } = true;

    /// <summary>
    ///     Indicates whether this collider is in contact with the other one.
    /// </summary>
    public bool IsColliding => PhysicsBodyProxy is not null && PhysicsBodyProxy.ContactCount > 0;

    // TODO: Add/update documentation for new APIs.
    // TODO: Allocation free APIs reuse existing buffers provided by the caller, avoiding allocations on each call.
    //       The buffer indirectly keeps managed references that prevents GC from collecting the colliders involved in contacts, which is a common issue when using structs that contain references.
    //       This trade-off should be documented that manual buffer clearing is required to allow GC to collect related objects.
    // TODO: Convert usages of GetContacts (allocating) to use new APIs.
    // TODO: Revisit whether GetContacts that allocates should be kept or removed in favor of APIs that do not allocate.
    // TODO: Update documentation after the Contact2D was changed to be a struct.
    public int ContactCount => PhysicsBodyProxy?.ContactCount ?? 0;

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
    public Contact2D[] GetContacts()
    {
        if (PhysicsBodyProxy is null)
        {
            return Array.Empty<Contact2D>();
        }

        var contacts = new Contact2D[PhysicsBodyProxy.ContactCount];
        PhysicsBodyProxy.GetContacts(contacts);

        return contacts;
    }

    public int GetContacts(Span<Contact2D> contacts) => PhysicsBodyProxy?.GetContacts(contacts) ?? 0;

    public int GetContacts(List<Contact2D> contacts)
    {
        while (contacts.Count < ContactCount)
        {
            contacts.Add(default);
        }

        return GetContacts(CollectionsMarshal.AsSpan(contacts));
    }

    public ReadOnlySpan<Contact2D> GetContactsAsSpan(Span<Contact2D> contacts)
    {
        var written = GetContacts(contacts);
        return contacts.Slice(0, written);
    }

    public ReadOnlySpan<Contact2D> GetContactsAsSpan(List<Contact2D> contacts)
    {
        var written = GetContacts(contacts);
        return CollectionsMarshal.AsSpan(contacts).Slice(0, written);
    }

    /// <inheritdoc />
    protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
    {
        base.Serialize(writer, assetStore);
        writer.WriteBool("Enabled", Enabled);
    }

    /// <inheritdoc />
    protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
    {
        base.Deserialize(reader, assetStore);
        Enabled = reader.ReadBool("Enabled");
    }
}