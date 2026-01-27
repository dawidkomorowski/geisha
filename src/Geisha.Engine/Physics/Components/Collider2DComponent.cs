using System;
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

    // TODO: Add XML documentation.
    // TODO: Test Tile collider when disabled and position misaligned but then enabled.
    // TODO: Test Tile collider SetTileCollider when enabled/disabled.
    // TODO: Add test cases for enabled/disabled colliders in StateSynchronizationTests.
    // TODO: Add microbenchmark for enabled/disabled colliders?
    public bool Enabled { get; set; } = true;

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

    protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
    {
        base.Serialize(writer, assetStore);
        writer.WriteBool("Enabled", Enabled);
    }

    protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
    {
        base.Deserialize(reader, assetStore);
        Enabled = reader.ReadBool("Enabled");
    }
}