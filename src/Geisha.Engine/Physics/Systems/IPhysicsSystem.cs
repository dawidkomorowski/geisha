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

    int QueryPoint(in Vector2 point, Span<Collider2DComponent> colliders);
    int QueryPoint(in Vector2 point, List<Collider2DComponent> colliders);
    ReadOnlySpan<Collider2DComponent> QueryPointAsSpan(in Vector2 point, Span<Collider2DComponent> colliders);
    ReadOnlySpan<Collider2DComponent> QueryPointAsSpan(in Vector2 point, List<Collider2DComponent> colliders);

    int QueryBounds(in AxisAlignedRectangle axisAlignedRectangle, Span<Collider2DComponent> colliders);
    int QueryBounds(in AxisAlignedRectangle axisAlignedRectangle, List<Collider2DComponent> colliders);
    ReadOnlySpan<Collider2DComponent> QueryBoundsAsSpan(in AxisAlignedRectangle axisAlignedRectangle, Span<Collider2DComponent> colliders);
    ReadOnlySpan<Collider2DComponent> QueryBoundsAsSpan(in AxisAlignedRectangle axisAlignedRectangle, List<Collider2DComponent> colliders);

    int QueryOverlap(in AxisAlignedRectangle axisAlignedRectangle, Span<Collider2DComponent> colliders);
    int QueryOverlap(in AxisAlignedRectangle axisAlignedRectangle, List<Collider2DComponent> colliders);
    ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in AxisAlignedRectangle axisAlignedRectangle, Span<Collider2DComponent> colliders);
    ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in AxisAlignedRectangle axisAlignedRectangle, List<Collider2DComponent> colliders);

    int QueryOverlap(in Circle circle, Span<Collider2DComponent> colliders);
    int QueryOverlap(in Circle circle, List<Collider2DComponent> colliders);
    ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in Circle circle, Span<Collider2DComponent> colliders);
    ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in Circle circle, List<Collider2DComponent> colliders);

    int QueryOverlap(in Rectangle rectangle, Span<Collider2DComponent> colliders);
    int QueryOverlap(in Rectangle rectangle, List<Collider2DComponent> colliders);
    ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in Rectangle rectangle, Span<Collider2DComponent> colliders);
    ReadOnlySpan<Collider2DComponent> QueryOverlapAsSpan(in Rectangle rectangle, List<Collider2DComponent> colliders);
}