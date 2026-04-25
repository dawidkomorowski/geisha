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

    /// <summary>
    ///     Gets the number of contacts currently involving this collider.
    /// </summary>
    /// <remarks>
    ///     Use <see cref="ContactCount" /> to pre-size a buffer before calling one of the non-allocating
    ///     <c>GetContacts</c> overloads, or to check whether contacts exist without retrieving them.
    ///     For a simple yes/no check prefer <see cref="IsColliding" />.
    /// </remarks>
    public int ContactCount => PhysicsBodyProxy?.ContactCount ?? 0;

    /// <summary>
    ///     Writes all current contacts involving this collider into the provided <paramref name="contacts" /> span and
    ///     returns the number of contacts written.
    /// </summary>
    /// <param name="contacts">
    ///     A caller-provided span that receives the current contacts. The span must be large enough to hold all contacts;
    ///     use <see cref="ContactCount" /> to determine the required size. If the span is smaller than the current contact
    ///     count, only as many contacts as fit in the span are written.
    /// </param>
    /// <returns>
    ///     The number of <see cref="Contact2D" /> values written into <paramref name="contacts" />, which is
    ///     <c>Min(<see cref="ContactCount" />, contacts.Length)</c>.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This overload does not allocate. It writes contacts directly into a caller-provided buffer, making it
    ///         suitable for use in performance-sensitive code paths.
    ///     </para>
    ///     <para>
    ///         If you are only interested in whether the collider is colliding with any other collider, use
    ///         <see cref="IsColliding" /> instead for better performance.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> <see cref="Contact2D" /> is a struct that contains managed references
    ///         (e.g. <see cref="Contact2D.ThisCollider" />, <see cref="Contact2D.OtherCollider" />). As long as the
    ///         caller's buffer is alive and populated, those references prevent the GC from collecting the referenced
    ///         colliders and the objects reachable from them. This is rarely a concern in scenes where entities persist
    ///         for the lifetime of the scene, but in transition scenarios — such as reloading a level or destroying
    ///         entities — the buffer should be cleared (e.g. <c>contacts.Clear()</c> for a <see cref="List{T}" />) to
    ///         release those references. It is not necessary to clear the buffer on every call or every frame.
    ///     </para>
    /// </remarks>
    public int GetContacts(Span<Contact2D> contacts) => PhysicsBodyProxy?.GetContacts(contacts) ?? 0;

    /// <summary>
    ///     Writes all current contacts involving this collider into the provided <paramref name="contacts" /> list and
    ///     returns the number of contacts written.
    /// </summary>
    /// <param name="contacts">
    ///     A caller-provided list that receives the current contacts. If the list contains fewer elements than
    ///     <see cref="ContactCount" />, the list is expanded to fit all contacts. Elements beyond the written range
    ///     are left unchanged; the list is never trimmed.
    /// </param>
    /// <returns>
    ///     The number of <see cref="Contact2D" /> values written into <paramref name="contacts" />, which equals
    ///     <see cref="ContactCount" />.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This overload does not allocate when the list already has sufficient capacity, making it suitable for
    ///         use in performance-sensitive code paths where the list is reused across calls.
    ///     </para>
    ///     <para>
    ///         If you are only interested in whether the collider is colliding with any other collider, use
    ///         <see cref="IsColliding" /> instead for better performance.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> <see cref="Contact2D" /> is a struct that contains managed references
    ///         (e.g. <see cref="Contact2D.ThisCollider" />, <see cref="Contact2D.OtherCollider" />). As long as the
    ///         list is alive and populated, those references prevent the GC from collecting the referenced colliders
    ///         and the objects reachable from them. This is rarely a concern in scenes where entities persist for the
    ///         lifetime of the scene, but in transition scenarios — such as reloading a level or destroying entities —
    ///         the list should be cleared (e.g. <c>contacts.Clear()</c>) to release those references. It is not
    ///         necessary to clear the list on every call or every frame.
    ///     </para>
    /// </remarks>
    public int GetContacts(List<Contact2D> contacts)
    {
        while (contacts.Count < ContactCount)
        {
            contacts.Add(default);
        }

        return GetContacts(CollectionsMarshal.AsSpan(contacts));
    }

    /// <summary>
    ///     Writes all current contacts involving this collider into the provided <paramref name="contacts" /> span and
    ///     returns a <see cref="ReadOnlySpan{T}" /> view over exactly the written elements.
    /// </summary>
    /// <param name="contacts">
    ///     A caller-provided span that receives the current contacts. The span must be large enough to hold all contacts;
    ///     use <see cref="ContactCount" /> to determine the required size. If the span is smaller than the current contact
    ///     count, only as many contacts as fit in the span are written.
    /// </param>
    /// <returns>
    ///     A <see cref="ReadOnlySpan{T}" /> sliced to the number of contacts written, allowing direct
    ///     <see langword="foreach" /> iteration over only the populated elements.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This overload does not allocate. It is equivalent to calling <see cref="GetContacts(Span{Contact2D})" />
    ///         and slicing the span to the returned count, but provides a more convenient iteration pattern.
    ///     </para>
    ///     <para>
    ///         The returned <see cref="ReadOnlySpan{T}" /> is valid only for the lifetime of the
    ///         <paramref name="contacts" /> buffer and must be consumed synchronously. It must not be stored beyond the
    ///         current stack frame, and cannot be used across <see langword="await" /> boundaries.
    ///     </para>
    ///     <para>
    ///         If you are only interested in whether the collider is colliding with any other collider, use
    ///         <see cref="IsColliding" /> instead for better performance.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> <see cref="Contact2D" /> is a struct that contains managed references
    ///         (e.g. <see cref="Contact2D.ThisCollider" />, <see cref="Contact2D.OtherCollider" />). As long as the
    ///         caller's buffer is alive and populated, those references prevent the GC from collecting the referenced
    ///         colliders and the objects reachable from them. This is rarely a concern in scenes where entities persist
    ///         for the lifetime of the scene, but in transition scenarios — such as reloading a level or destroying
    ///         entities — the buffer should be cleared to release those references. It is not necessary to clear the
    ///         buffer on every call or every frame.
    ///     </para>
    /// </remarks>
    public ReadOnlySpan<Contact2D> GetContactsAsSpan(Span<Contact2D> contacts)
    {
        var written = GetContacts(contacts);
        return contacts.Slice(0, written);
    }

    /// <summary>
    ///     Writes all current contacts involving this collider into the provided <paramref name="contacts" /> list and
    ///     returns a <see cref="ReadOnlySpan{T}" /> view over exactly the written elements.
    /// </summary>
    /// <param name="contacts">
    ///     A caller-provided list that receives the current contacts. If the list contains fewer elements than
    ///     <see cref="ContactCount" />, the list is expanded to fit all contacts. Elements beyond the written range
    ///     are left unchanged; the list is never trimmed.
    /// </param>
    /// <returns>
    ///     A <see cref="ReadOnlySpan{T}" /> sliced to the number of contacts written, allowing direct
    ///     <see langword="foreach" /> iteration over only the populated elements.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This overload does not allocate when the list already has sufficient capacity. It is equivalent to
    ///         calling <see cref="GetContacts(List{Contact2D})" /> and slicing the result to the returned count, but
    ///         provides a more convenient iteration pattern.
    ///     </para>
    ///     <para>
    ///         The returned <see cref="ReadOnlySpan{T}" /> is backed by the list's internal array and is valid only as
    ///         long as the list is not resized and must be consumed synchronously. It must not be stored beyond the
    ///         current stack frame, and cannot be used across <see langword="await" /> boundaries.
    ///     </para>
    ///     <para>
    ///         If you are only interested in whether the collider is colliding with any other collider, use
    ///         <see cref="IsColliding" /> instead for better performance.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> <see cref="Contact2D" /> is a struct that contains managed references
    ///         (e.g. <see cref="Contact2D.ThisCollider" />, <see cref="Contact2D.OtherCollider" />). As long as the
    ///         list is alive and populated, those references prevent the GC from collecting the referenced colliders
    ///         and the objects reachable from them. This is rarely a concern in scenes where entities persist for the
    ///         lifetime of the scene, but in transition scenarios — such as reloading a level or destroying entities —
    ///         the list should be cleared (e.g. <c>contacts.Clear()</c>) to release those references. It is not
    ///         necessary to clear the list on every call or every frame.
    ///     </para>
    /// </remarks>
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