using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.Math;
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
    // TODO: This could be replaced with field keyword in .NET 10 (C# 14).
    // TODO: When field keyword is used, consider searching for other places in the codebase where it could be used as well.
    private bool _enabled = true;
    private bool _isSensor = false;
    private CollisionBitmask _collisionLayer = CollisionBitmask.All;
    private CollisionBitmask _collisionMask = CollisionBitmask.All;

    internal PhysicsBodyProxy? PhysicsBodyProxy { get; set; }
    internal bool IsDirty { get; set; } = true;

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
    public bool Enabled
    {
        get => _enabled;
        set
        {
            _enabled = value;
            IsDirty = true;
        }
    }

    /// <summary>
    ///     Gets or sets a value indicating whether this collider behaves as a sensor.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A sensor collider participates in overlap detection but is intended for trigger-like behavior via
    ///         overlap callbacks, rather than physical collision response.
    ///     </para>
    ///     <para>
    ///         Sensors do not produce contacts with other colliders. As a result, sensor overlaps are not reported
    ///         through regular contact-based collision detection APIs (for example <see cref="IsColliding" />,
    ///         <see cref="ContactCount" />, or <c>GetContacts</c> overloads) and do not generate collision response.
    ///     </para>
    ///     <para>
    ///         Sensor overlap notifications are reported through <see cref="OnOverlapBegin" /> and
    ///         <see cref="OnOverlapEnd" />.
    ///     </para>
    ///     <para>
    ///         The default value is <see langword="false"/>.
    ///     </para>
    /// </remarks>
    public bool IsSensor
    {
        get => _isSensor;
        set
        {
            _isSensor = value;
            IsDirty = true;
        }
    }

    /// <summary>
    ///     Gets or sets a callback invoked when this collider begins an overlap that involves at least one sensor collider.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The callback receives the other collider in the overlap pair.
    ///     </para>
    ///     <para>
    ///         Overlap callbacks are generated for overlaps that involve at least one sensor collider, and both
    ///         colliders in such pair receive notifications. For example, when a sensor overlaps a non-sensor,
    ///         both colliders are notified.
    ///     </para>
    ///     <para>
    ///         Callback invocation happens as the final part of a physics simulation step, after simulation and
    ///         collision solving complete.
    ///     </para>
    ///     <para>
    ///         For a given pair of colliders, the begin callback is guaranteed to be invoked before the corresponding
    ///         <see cref="OnOverlapEnd" /> callback.
    ///     </para>
    ///     <para>
    ///         Across different overlap pairs in the same frame, invocation order is unspecified.
    ///     </para>
    /// </remarks>
    public Action<Collider2DComponent>? OnOverlapBegin { get; set; }

    /// <summary>
    ///     Gets or sets a callback invoked when this collider ends an overlap that involves at least one sensor collider.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The callback receives the other collider in the overlap pair.
    ///     </para>
    ///     <para>
    ///         Overlap callbacks are generated for overlaps that involve at least one sensor collider, and both
    ///         colliders in such pair receive notifications. For example, when a sensor overlaps a non-sensor,
    ///         both colliders are notified.
    ///     </para>
    ///     <para>
    ///         Callback invocation happens as the final part of a physics simulation step, after simulation and
    ///         collision solving complete.
    ///     </para>
    ///     <para>
    ///         For a given pair of colliders, the corresponding <see cref="OnOverlapBegin" /> callback is guaranteed
    ///         to be invoked before this end callback.
    ///     </para>
    ///     <para>
    ///         Across different overlap pairs in the same frame, invocation order is unspecified.
    ///     </para>
    /// </remarks>
    public Action<Collider2DComponent>? OnOverlapEnd { get; set; }

    /// <summary>
    ///     Gets or sets the collision layer bitmask that identifies which logical groups this collider belongs to.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="CollisionLayer" /> describes what this collider <i>is</i>. Each set bit marks membership in a
    ///         corresponding collision layer.
    ///     </para>
    ///     <para>
    ///         During collision filtering between colliders A and B, this value is matched against B's
    ///         <see cref="CollisionMask" />. A collision pair is accepted only when both directional checks pass:
    ///         (A.Layer &amp; B.Mask) != 0 and (A.Mask &amp; B.Layer) != 0.
    ///     </para>
    ///     <para>
    ///         Default value is <see cref="CollisionBitmask.All" />, meaning this collider belongs to all layers.
    ///     </para>
    /// </remarks>
    /// <seealso cref="CollisionMask" />
    /// <seealso cref="CollisionBitmask" />
    public CollisionBitmask CollisionLayer
    {
        get => _collisionLayer;
        set
        {
            _collisionLayer = value;
            IsDirty = true;
        }
    }

    /// <summary>
    ///     Gets or sets the collision mask bitmask that defines which layers this collider can collide with.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="CollisionMask" /> describes what this collider collides <i>with</i>. Each set bit allows
    ///         collisions with colliders that have the same bit set in their <see cref="CollisionLayer" />.
    ///     </para>
    ///     <para>
    ///         During collision filtering between colliders A and B, this value is matched against B's
    ///         <see cref="CollisionLayer" />. A collision pair is accepted only when both directional checks pass:
    ///         (A.Layer &amp; B.Mask) != 0 and (A.Mask &amp; B.Layer) != 0.
    ///     </para>
    ///     <para>
    ///         Default value is <see cref="CollisionBitmask.All" />, meaning this collider accepts collisions with all
    ///         layers.
    ///     </para>
    /// </remarks>
    /// <seealso cref="CollisionLayer" />
    /// <seealso cref="CollisionBitmask" />
    public CollisionBitmask CollisionMask
    {
        get => _collisionMask;
        set
        {
            _collisionMask = value;
            IsDirty = true;
        }
    }

    /// <summary>
    ///     Gets the axis-aligned bounding rectangle of this collider as computed at the last physics simulation step.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The bounding rectangle is an <see cref="AxisAlignedRectangle" /> that fully encloses the collider geometry
    ///         in world space, taking into account the collider's shape, position, and rotation.
    ///     </para>
    ///     <para>
    ///         <b>Synchronization behavior:</b> This property reflects the state from the most recently completed physics
    ///         simulation step. Any changes made to the entity's <see cref="Core.Components.Transform2DComponent" /> or
    ///         to the collider's shape properties (such as <see cref="CircleColliderComponent.Radius" /> or
    ///         <see cref="RectangleColliderComponent.Dimensions" />) after the last simulation step will not be reflected
    ///         until the next physics simulation step completes. This means that within a single game loop iteration, if
    ///         game code modifies the transform or collider and then immediately reads <see cref="BoundingRectangle" />,
    ///         the returned value will still correspond to the state before the modification.
    ///     </para>
    ///     <para>
    ///         <b>Before first simulation step:</b> When the physics body is first registered — that is, when both
    ///         <see cref="Core.Components.Transform2DComponent" /> and the collider component are present on the entity
    ///         — an initial value is computed from the component state at that moment. This means the value depends on
    ///         the order in which components are created and configured: properties set before the physics body is
    ///         registered are reflected immediately, while properties set after are not visible until the first
    ///         simulation step completes. If <see cref="Core.Components.Transform2DComponent" /> is not yet present on
    ///         the entity, this property returns a <see langword="default" /> <see cref="AxisAlignedRectangle" />.
    ///     </para>
    ///     <para>
    ///         <b>Consistency across entities:</b> Because all values are taken from the same completed simulation step,
    ///         reading <see cref="BoundingRectangle" /> from multiple colliders within the same game loop iteration
    ///         provides a globally consistent snapshot of the physics state, unaffected by the order in which individual
    ///         entities are updated during that iteration.
    ///     </para>
    /// </remarks>
    /// <seealso cref="SynchronizePhysicsState" />
    public AxisAlignedRectangle BoundingRectangle => PhysicsBodyProxy?.BoundingRectangle ?? default;

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

    /// <summary>
    ///     Determines whether the specified point in world space is contained within this collider's geometry.
    /// </summary>
    /// <param name="point">The point in world space to test.</param>
    /// <returns>
    ///     <see langword="true" /> if the point is contained within this collider; otherwise, <see langword="false" />.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This method performs a geometric containment test against the actual collider shape, not just
    ///         <see cref="BoundingRectangle" /> containment.
    ///     </para>
    ///     <para>
    ///         This query observes collider state from the most recently synchronized physics data. If transform or
    ///         collider properties were modified after the last synchronization, the result may reflect stale state until
    ///         the next physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         If this collider is not yet managed by the physics system, this method returns <see langword="false" />.
    ///     </para>
    /// </remarks>
    public bool ContainsPoint(in Vector2 point) => PhysicsBodyProxy?.ContainsPoint(point) ?? false;

    /// <summary>
    ///     Determines whether this collider overlaps with the specified axis-aligned rectangle.
    /// </summary>
    /// <param name="axisAlignedRectangle">The axis-aligned rectangle to test for overlap.</param>
    /// <returns>
    ///     <see langword="true" /> if this collider overlaps with the rectangle; otherwise, <see langword="false" />.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This query observes collider state from the most recently synchronized physics data. If transform or
    ///         collider properties were modified after the last synchronization, the result may reflect stale state until
    ///         the next physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         If this collider is not yet managed by the physics system, this method returns <see langword="false" />.
    ///     </para>
    /// </remarks>
    /// <seealso cref="Overlaps(in Circle)" />
    /// <seealso cref="Overlaps(in Rectangle)" />
    public bool Overlaps(in AABB2D aabb) => PhysicsBodyProxy?.Overlaps(aabb) ?? false;

    /// <summary>
    ///     Determines whether this collider overlaps with the specified circle.
    /// </summary>
    /// <param name="circle">The circle to test for overlap.</param>
    /// <returns>
    ///     <see langword="true" /> if this collider overlaps with the circle; otherwise, <see langword="false" />.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This query observes collider state from the most recently synchronized physics data. If transform or
    ///         collider properties were modified after the last synchronization, the result may reflect stale state until
    ///         the next physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         If this collider is not yet managed by the physics system, this method returns <see langword="false" />.
    ///     </para>
    /// </remarks>
    /// <seealso cref="Overlaps(in AxisAlignedRectangle)" />
    /// <seealso cref="Overlaps(in Rectangle)" />
    public bool Overlaps(in Circle circle) => PhysicsBodyProxy?.Overlaps(circle) ?? false;

    /// <summary>
    ///     Determines whether this collider overlaps with the specified rotated rectangle.
    /// </summary>
    /// <param name="rectangle">The rotated rectangle to test for overlap.</param>
    /// <returns>
    ///     <see langword="true" /> if this collider overlaps with the rectangle; otherwise, <see langword="false" />.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This query observes collider state from the most recently synchronized physics data. If transform or
    ///         collider properties were modified after the last synchronization, the result may reflect stale state until
    ///         the next physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         If this collider is not yet managed by the physics system, this method returns <see langword="false" />.
    ///     </para>
    /// </remarks>
    /// <seealso cref="Overlaps(in AxisAlignedRectangle)" />
    /// <seealso cref="Overlaps(in Circle)" />
    public bool Overlaps(in Rectangle rectangle) => PhysicsBodyProxy?.Overlaps(rectangle) ?? false;

    /// <summary>
    ///     Immediately synchronizes this collider's physics body with the current component state.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Calling this method pushes the current state of the entity's physics components —
    ///         <see cref="Core.Components.Transform2DComponent" />, this collider's properties, and
    ///         <see cref="KinematicRigidBody2DComponent" /> if present — into the underlying physics body, making those
    ///         values visible to physics-related APIs such as <see cref="BoundingRectangle" /> before the next physics
    ///         simulation step runs.
    ///     </para>
    ///     <para>
    ///         <b>Typical use case:</b> When an entity's transform or collider properties are modified and the updated
    ///         physics state must be observed in the same game loop iteration — for example, reading
    ///         <see cref="BoundingRectangle" /> immediately after repositioning an entity — call this method after the
    ///         modification to ensure the physics body reflects the new state.
    ///     </para>
    ///     <para>
    ///         <b>Granularity:</b> Unlike <see cref="IPhysicsSystem.SynchronizePhysicsState" />, which synchronizes all
    ///         registered physics bodies at once, this method affects only the single collider it is called on. This
    ///         allows targeted synchronization of individual entities without touching the rest of the physics state.
    ///     </para>
    ///     <para>
    ///         <b>When not needed:</b> During normal gameplay the physics system synchronizes all bodies automatically at
    ///         each simulation step. This method is only necessary when physics state must be up to date before the
    ///         next simulation step.
    ///     </para>
    ///     <para>
    ///         If this collider is not yet managed by the physics system (i.e. it has no associated physics body),
    ///         calling this method has no effect.
    ///     </para>
    /// </remarks>
    /// <seealso cref="IPhysicsSystem.SynchronizePhysicsState" />
    public void SynchronizePhysicsState() => PhysicsBodyProxy?.SynchronizeBody();

    /// <inheritdoc />
    protected internal override void Serialize(IComponentDataWriter writer, IAssetStore assetStore)
    {
        base.Serialize(writer, assetStore);
        writer.WriteBool("Enabled", Enabled);
        writer.WriteBool("IsSensor", IsSensor);
        writer.WriteUInt("CollisionLayer", CollisionLayer.Value);
        writer.WriteUInt("CollisionMask", CollisionMask.Value);
    }

    /// <inheritdoc />
    protected internal override void Deserialize(IComponentDataReader reader, IAssetStore assetStore)
    {
        base.Deserialize(reader, assetStore);
        Enabled = reader.ReadBool("Enabled");
        IsSensor = reader.ReadBool("IsSensor");
        CollisionLayer = new CollisionBitmask(reader.ReadUInt("CollisionLayer"));
        CollisionMask = new CollisionBitmask(reader.ReadUInt("CollisionMask"));
    }
}