using System;
using System.Collections.Generic;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Physics.Components;

namespace Geisha.Engine.Physics.Systems;

/// <summary>
///     Provides access to runtime settings and controls of the physics system.
/// </summary>
/// <seealso cref="Geisha.Engine.Physics.PhysicsConfiguration" />
public interface IPhysicsSystem
{
    /// <summary>
    ///     Gets or sets a value indicating whether physics debug rendering is enabled.
    /// </summary>
    /// <remarks>
    ///     When enabled, the physics system renders debug visualizations (for example, collision geometry) on top of the
    ///     standard graphics output to assist with debugging.
    /// </remarks>
    bool EnableDebugRendering { get; set; }

    /// <summary>
    ///     Immediately synchronizes all registered physics bodies with the current component state.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Calling this method pushes the current state of each entity's physics components —
    ///         <see cref="Geisha.Engine.Core.Components.Transform2DComponent" />, collider properties, and
    ///         <see cref="KinematicRigidBody2DComponent" /> if present — into their
    ///         underlying physics bodies, making those values visible to physics-related APIs such as
    ///         <see cref="Collider2DComponent.BoundingRectangle" /> before the next
    ///         physics simulation step runs.
    ///     </para>
    ///     <para>
    ///         <b>Typical use case:</b> After setting up a scene — for example, loading a game level and positioning
    ///         all entities — call this method once to ensure that physics-related APIs such as
    ///         <see cref="Collider2DComponent.BoundingRectangle" /> return correctly initialized values from the very first
    ///         frame the scene is processed, before the physics simulation step has had a chance to run.
    ///     </para>
    ///     <para>
    ///         <b>Granularity:</b> This method synchronizes all registered physics bodies at once. To synchronize
    ///         only a single collider, use <see cref="Collider2DComponent.SynchronizePhysicsState" /> on the
    ///         individual component instead.
    ///     </para>
    ///     <para>
    ///         <b>When not needed:</b> During normal gameplay the physics system synchronizes all bodies automatically
    ///         at each simulation step. This method is only necessary when physics state must be up to date before
    ///         the next simulation step.
    ///     </para>
    /// </remarks>
    /// <seealso cref="Collider2DComponent.SynchronizePhysicsState" />
    void SynchronizePhysicsState();

    /// <summary>
    ///     Finds colliders that contain the specified point and writes them into the provided
    ///     <paramref name="colliders" /> span.
    /// </summary>
    /// <param name="point">Point in world space to test.</param>
    /// <param name="colliders">
    ///     Destination span for query results. If fewer elements are available than matching colliders, only the
    ///     colliders that fit are written.
    /// </param>
    /// <returns>
    ///     Number of colliders written to <paramref name="colliders" />, up to <c>colliders.Length</c>.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed. Clearing on every query or every frame is typically unnecessary.
    ///     </para>
    /// </remarks>
    int QueryPoint(in Vector2 point, Span<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders that contain the specified point and writes them into the provided
    ///     <paramref name="colliders" /> list.
    /// </summary>
    /// <param name="point">Point in world space to test.</param>
    /// <param name="colliders">
    ///     Destination list for query results. The list is cleared before writing the results.
    /// </param>
    /// <returns>Number of colliders written to <paramref name="colliders" />.</returns>
    /// <remarks>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed (for example, <c>colliders.Clear()</c>). Clearing on every query or every frame is typically
    ///         unnecessary.
    ///     </para>
    /// </remarks>
    int QueryPoint(in Vector2 point, List<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders that contain the specified point, writes them into the provided
    ///     <paramref name="colliders" /> span, and returns a view over the written range.
    /// </summary>
    /// <param name="point">Point in world space to test.</param>
    /// <param name="colliders">
    ///     Destination span for query results. If fewer elements are available than matching colliders, only the
    ///     colliders that fit are written.
    /// </param>
    /// <returns>
    ///     A <see cref="ReadOnlySpan{T}" /> view over the written portion of <paramref name="colliders" />.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This overload is equivalent to calling <see cref="QueryPoint(in Vector2, Span{Collider2DComponent})" />
    ///         and slicing the buffer to the written count.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed. Clearing on every query or every frame is typically unnecessary.
    ///     </para>
    /// </remarks>
    ReadOnlySpan<Collider2DComponent> QueryPointAsSpan(in Vector2 point, Span<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders that contain the specified point, writes them into the provided
    ///     <paramref name="colliders" /> list, and returns a view over the written range.
    /// </summary>
    /// <param name="point">Point in world space to test.</param>
    /// <param name="colliders">
    ///     Destination list for query results. The list is cleared before writing the results.
    /// </param>
    /// <returns>
    ///     A <see cref="ReadOnlySpan{T}" /> view over the written portion of <paramref name="colliders" />.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This overload is equivalent to calling <see cref="QueryPoint(in Vector2, List{Collider2DComponent})" />
    ///         and slicing the list to the written count.
    ///     </para>
    ///     <para>
    ///         The returned span is backed by the list's internal storage and is valid only while the list is not
    ///         resized.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed (for example, <c>colliders.Clear()</c>). Clearing on every query or every frame is typically
    ///         unnecessary.
    ///     </para>
    /// </remarks>
    ReadOnlySpan<Collider2DComponent> QueryPointAsSpan(in Vector2 point, List<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders whose bounding rectangles overlap the specified axis-aligned rectangle and writes them into
    ///     the provided <paramref name="colliders" /> span.
    /// </summary>
    /// <param name="axisAlignedRectangle">Axis-aligned rectangle in world space to test.</param>
    /// <param name="colliders">
    ///     Destination span for query results. If fewer elements are available than matching colliders, only the
    ///     colliders that fit are written.
    /// </param>
    /// <returns>
    ///     Number of colliders written to <paramref name="colliders" />, up to <c>colliders.Length</c>.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This method tests overlap against collider bounding rectangles
    ///         (<see cref="Collider2DComponent.BoundingRectangle" />), not exact collider geometry.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed. Clearing on every query or every frame is typically unnecessary.
    ///     </para>
    /// </remarks>
    int QueryBounds(in AxisAlignedRectangle axisAlignedRectangle, Span<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders whose bounding rectangles overlap the specified axis-aligned rectangle and writes them into
    ///     the provided <paramref name="colliders" /> list.
    /// </summary>
    /// <param name="axisAlignedRectangle">Axis-aligned rectangle in world space to test.</param>
    /// <param name="colliders">
    ///     Destination list for query results. The list is cleared before writing the results.
    /// </param>
    /// <returns>Number of colliders written to <paramref name="colliders" />.</returns>
    /// <remarks>
    ///     <para>
    ///         This method tests overlap against collider bounding rectangles
    ///         (<see cref="Collider2DComponent.BoundingRectangle" />), not exact collider geometry.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed (for example, <c>colliders.Clear()</c>). Clearing on every query or every frame is typically
    ///         unnecessary.
    ///     </para>
    /// </remarks>
    int QueryBounds(in AxisAlignedRectangle axisAlignedRectangle, List<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders whose bounding rectangles overlap the specified axis-aligned rectangle, writes them into
    ///     the provided <paramref name="colliders" /> span, and returns a view over the written range.
    /// </summary>
    /// <param name="axisAlignedRectangle">Axis-aligned rectangle in world space to test.</param>
    /// <param name="colliders">
    ///     Destination span for query results. If fewer elements are available than matching colliders, only the
    ///     colliders that fit are written.
    /// </param>
    /// <returns>
    ///     A <see cref="ReadOnlySpan{T}" /> view over the written portion of <paramref name="colliders" />.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This overload is equivalent to calling
    ///         <see cref="QueryBounds(in AxisAlignedRectangle, Span{Collider2DComponent})" /> and slicing the buffer
    ///         to the written count.
    ///     </para>
    ///     <para>
    ///         This method tests overlap against collider bounding rectangles
    ///         (<see cref="Collider2DComponent.BoundingRectangle" />), not exact collider geometry.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed. Clearing on every query or every frame is typically unnecessary.
    ///     </para>
    /// </remarks>
    ReadOnlySpan<Collider2DComponent> QueryBoundsAsSpan(in AxisAlignedRectangle axisAlignedRectangle, Span<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders whose bounding rectangles overlap the specified axis-aligned rectangle, writes them into
    ///     the provided <paramref name="colliders" /> list, and returns a view over the written range.
    /// </summary>
    /// <param name="axisAlignedRectangle">Axis-aligned rectangle in world space to test.</param>
    /// <param name="colliders">
    ///     Destination list for query results. The list is cleared before writing the results.
    /// </param>
    /// <returns>
    ///     A <see cref="ReadOnlySpan{T}" /> view over the written portion of <paramref name="colliders" />.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This overload is equivalent to calling
    ///         <see cref="QueryBounds(in AxisAlignedRectangle, List{Collider2DComponent})" /> and slicing the list to
    ///         the written count.
    ///     </para>
    ///     <para>
    ///         The returned span is backed by the list's internal storage and is valid only while the list is not
    ///         resized.
    ///     </para>
    ///     <para>
    ///         This method tests overlap against collider bounding rectangles
    ///         (<see cref="Collider2DComponent.BoundingRectangle" />), not exact collider geometry.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed (for example, <c>colliders.Clear()</c>). Clearing on every query or every frame is typically
    ///         unnecessary.
    ///     </para>
    /// </remarks>
    ReadOnlySpan<Collider2DComponent> QueryBoundsAsSpan(in AxisAlignedRectangle axisAlignedRectangle, List<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders that overlap the specified axis-aligned rectangle and writes them into the provided
    ///     <paramref name="colliders" /> span.
    /// </summary>
    /// <param name="axisAlignedRectangle">Axis-aligned rectangle in world space to test for overlap.</param>
    /// <param name="colliders">
    ///     Destination span for query results. If fewer elements are available than matching colliders, only the
    ///     colliders that fit are written.
    /// </param>
    /// <returns>
    ///     Number of colliders written to <paramref name="colliders" />, up to <c>colliders.Length</c>.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This query performs geometric overlap tests against collider shapes.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed. Clearing on every query or every frame is typically unnecessary.
    ///     </para>
    /// </remarks>
    int QueryOverlap(in AxisAlignedRectangle axisAlignedRectangle, Span<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders that overlap the specified axis-aligned rectangle and writes them into the provided
    ///     <paramref name="colliders" /> list.
    /// </summary>
    /// <param name="axisAlignedRectangle">Axis-aligned rectangle in world space to test for overlap.</param>
    /// <param name="colliders">
    ///     Destination list for query results. The list is cleared before writing the results.
    /// </param>
    /// <returns>Number of colliders written to <paramref name="colliders" />.</returns>
    /// <remarks>
    ///     <para>
    ///         This query performs geometric overlap tests against collider shapes.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed (for example, <c>colliders.Clear()</c>). Clearing on every query or every frame is typically
    ///         unnecessary.
    ///     </para>
    /// </remarks>
    int QueryOverlap(in AxisAlignedRectangle axisAlignedRectangle, List<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders that overlap the specified axis-aligned rectangle, writes them into the provided
    ///     <paramref name="colliders" /> span, and returns a view over the written range.
    /// </summary>
    /// <param name="axisAlignedRectangle">Axis-aligned rectangle in world space to test for overlap.</param>
    /// <param name="colliders">
    ///     Destination span for query results. If fewer elements are available than matching colliders, only the
    ///     colliders that fit are written.
    /// </param>
    /// <returns>
    ///     A <see cref="ReadOnlySpan{T}" /> view over the written portion of <paramref name="colliders" />.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This overload is equivalent to calling
    ///         <see cref="QueryOverlap(in AxisAlignedRectangle, Span{Collider2DComponent})" /> and slicing the buffer
    ///         to the written count.
    ///     </para>
    ///     <para>
    ///         This query performs geometric overlap tests against collider shapes.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed. Clearing on every query or every frame is typically unnecessary.
    ///     </para>
    /// </remarks>
    ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in AxisAlignedRectangle axisAlignedRectangle, Span<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders that overlap the specified axis-aligned rectangle, writes them into the provided
    ///     <paramref name="colliders" /> list, and returns a view over the written range.
    /// </summary>
    /// <param name="axisAlignedRectangle">Axis-aligned rectangle in world space to test for overlap.</param>
    /// <param name="colliders">
    ///     Destination list for query results. The list is cleared before writing the results.
    /// </param>
    /// <returns>
    ///     A <see cref="ReadOnlySpan{T}" /> view over the written portion of <paramref name="colliders" />.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This overload is equivalent to calling
    ///         <see cref="QueryOverlap(in AxisAlignedRectangle, List{Collider2DComponent})" /> and slicing the list to
    ///         the written count.
    ///     </para>
    ///     <para>
    ///         The returned span is backed by the list's internal storage and is valid only while the list is not
    ///         resized.
    ///     </para>
    ///     <para>
    ///         This query performs geometric overlap tests against collider shapes.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed (for example, <c>colliders.Clear()</c>). Clearing on every query or every frame is typically
    ///         unnecessary.
    ///     </para>
    /// </remarks>
    ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in AxisAlignedRectangle axisAlignedRectangle, List<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders that overlap the specified circle and writes them into the provided
    ///     <paramref name="colliders" /> span.
    /// </summary>
    /// <param name="circle">Circle in world space to test for overlap.</param>
    /// <param name="colliders">
    ///     Destination span for query results. If fewer elements are available than matching colliders, only the
    ///     colliders that fit are written.
    /// </param>
    /// <returns>
    ///     Number of colliders written to <paramref name="colliders" />, up to <c>colliders.Length</c>.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This query performs geometric overlap tests against collider shapes.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed. Clearing on every query or every frame is typically unnecessary.
    ///     </para>
    /// </remarks>
    int QueryOverlap(in Circle circle, Span<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders that overlap the specified circle and writes them into the provided
    ///     <paramref name="colliders" /> list.
    /// </summary>
    /// <param name="circle">Circle in world space to test for overlap.</param>
    /// <param name="colliders">
    ///     Destination list for query results. The list is cleared before writing the results.
    /// </param>
    /// <returns>Number of colliders written to <paramref name="colliders" />.</returns>
    /// <remarks>
    ///     <para>
    ///         This query performs geometric overlap tests against collider shapes.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed (for example, <c>colliders.Clear()</c>). Clearing on every query or every frame is typically
    ///         unnecessary.
    ///     </para>
    /// </remarks>
    int QueryOverlap(in Circle circle, List<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders that overlap the specified circle, writes them into the provided
    ///     <paramref name="colliders" /> span, and returns a view over the written range.
    /// </summary>
    /// <param name="circle">Circle in world space to test for overlap.</param>
    /// <param name="colliders">
    ///     Destination span for query results. If fewer elements are available than matching colliders, only the
    ///     colliders that fit are written.
    /// </param>
    /// <returns>
    ///     A <see cref="ReadOnlySpan{T}" /> view over the written portion of <paramref name="colliders" />.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This overload is equivalent to calling <see cref="QueryOverlap(in Circle, Span{Collider2DComponent})" />
    ///         and slicing the buffer to the written count.
    ///     </para>
    ///     <para>
    ///         This query performs geometric overlap tests against collider shapes.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed. Clearing on every query or every frame is typically unnecessary.
    ///     </para>
    /// </remarks>
    ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in Circle circle, Span<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders that overlap the specified circle, writes them into the provided
    ///     <paramref name="colliders" /> list, and returns a view over the written range.
    /// </summary>
    /// <param name="circle">Circle in world space to test for overlap.</param>
    /// <param name="colliders">
    ///     Destination list for query results. The list is cleared before writing the results.
    /// </param>
    /// <returns>
    ///     A <see cref="ReadOnlySpan{T}" /> view over the written portion of <paramref name="colliders" />.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This overload is equivalent to calling <see cref="QueryOverlap(in Circle, List{Collider2DComponent})" />
    ///         and slicing the list to the written count.
    ///     </para>
    ///     <para>
    ///         The returned span is backed by the list's internal storage and is valid only while the list is not
    ///         resized.
    ///     </para>
    ///     <para>
    ///         This query performs geometric overlap tests against collider shapes.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed (for example, <c>colliders.Clear()</c>). Clearing on every query or every frame is typically
    ///         unnecessary.
    ///     </para>
    /// </remarks>
    ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in Circle circle, List<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders that overlap the specified rotated rectangle and writes them into the provided
    ///     <paramref name="colliders" /> span.
    /// </summary>
    /// <param name="rectangle">Rotated rectangle in world space to test for overlap.</param>
    /// <param name="colliders">
    ///     Destination span for query results. If fewer elements are available than matching colliders, only the
    ///     colliders that fit are written.
    /// </param>
    /// <returns>
    ///     Number of colliders written to <paramref name="colliders" />, up to <c>colliders.Length</c>.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This query performs geometric overlap tests against collider shapes.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed. Clearing on every query or every frame is typically unnecessary.
    ///     </para>
    /// </remarks>
    int QueryOverlap(in Rectangle rectangle, Span<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders that overlap the specified rotated rectangle and writes them into the provided
    ///     <paramref name="colliders" /> list.
    /// </summary>
    /// <param name="rectangle">Rotated rectangle in world space to test for overlap.</param>
    /// <param name="colliders">
    ///     Destination list for query results. The list is cleared before writing the results.
    /// </param>
    /// <returns>Number of colliders written to <paramref name="colliders" />.</returns>
    /// <remarks>
    ///     <para>
    ///         This query performs geometric overlap tests against collider shapes.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed (for example, <c>colliders.Clear()</c>). Clearing on every query or every frame is typically
    ///         unnecessary.
    ///     </para>
    /// </remarks>
    int QueryOverlap(in Rectangle rectangle, List<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders that overlap the specified rotated rectangle, writes them into the provided
    ///     <paramref name="colliders" /> span, and returns a view over the written range.
    /// </summary>
    /// <param name="rectangle">Rotated rectangle in world space to test for overlap.</param>
    /// <param name="colliders">
    ///     Destination span for query results. If fewer elements are available than matching colliders, only the
    ///     colliders that fit are written.
    /// </param>
    /// <returns>
    ///     A <see cref="ReadOnlySpan{T}" /> view over the written portion of <paramref name="colliders" />.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This overload is equivalent to calling
    ///         <see cref="QueryOverlap(in Rectangle, Span{Collider2DComponent})" /> and slicing the buffer to the
    ///         written count.
    ///     </para>
    ///     <para>
    ///         This query performs geometric overlap tests against collider shapes.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed. Clearing on every query or every frame is typically unnecessary.
    ///     </para>
    /// </remarks>
    ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in Rectangle rectangle, Span<Collider2DComponent> colliders);

    /// <summary>
    ///     Finds colliders that overlap the specified rotated rectangle, writes them into the provided
    ///     <paramref name="colliders" /> list, and returns a view over the written range.
    /// </summary>
    /// <param name="rectangle">Rotated rectangle in world space to test for overlap.</param>
    /// <param name="colliders">
    ///     Destination list for query results. The list is cleared before writing the results.
    /// </param>
    /// <returns>
    ///     A <see cref="ReadOnlySpan{T}" /> view over the written portion of <paramref name="colliders" />.
    /// </returns>
    /// <remarks>
    ///     <para>
    ///         This overload is equivalent to calling
    ///         <see cref="QueryOverlap(in Rectangle, List{Collider2DComponent})" /> and slicing the list to the
    ///         written count.
    ///     </para>
    ///     <para>
    ///         The returned span is backed by the list's internal storage and is valid only while the list is not
    ///         resized.
    ///     </para>
    ///     <para>
    ///         This query performs geometric overlap tests against collider shapes.
    ///     </para>
    ///     <para>
    ///         This query observes physics state from the most recently synchronized data. If transform or collider
    ///         properties were modified after the last synchronization, results may reflect stale state until the next
    ///         physics simulation step or until <see cref="SynchronizePhysicsState" /> is called.
    ///     </para>
    ///     <para>
    ///         <b>GC retention trade-off:</b> Keeping references written into a long-lived destination buffer can keep
    ///         removed collider components alive and may look like a memory leak. In scene transitions (for example,
    ///         level reloads or mass entity destruction), clear or overwrite stale entries when results are no longer
    ///         needed (for example, <c>colliders.Clear()</c>). Clearing on every query or every frame is typically
    ///         unnecessary.
    ///     </para>
    /// </remarks>
    ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in Rectangle rectangle, List<Collider2DComponent> colliders);
}