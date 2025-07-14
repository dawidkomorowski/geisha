using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;

namespace Geisha.Engine.Physics.Components;

/// <summary>
///     Tile collider component is used to create a tile-based collider in a 2D physics system. It is used to solve the
///     issue with ghost collisions between moving objects and a static tile-based geometry.
/// </summary>
/// <remarks>
///     <para>
///         Tile colliders are rectangular colliders that are aligned with the tile grid. All tile colliders in the single
///         scene have the same size. The size of the tile collider is defined by the physics configuration property
///         <see cref="PhysicsConfiguration.TileSize" />.
///     </para>
///     <para>
///         <see cref="TileColliderComponent" /> is mainly used to solve the issue with ghost collisions between moving
///         objects and a static tile-based geometry. Using <see cref="RectangleColliderComponent" /> to represent
///         tile-based geometry may result with ghost collisions on edges of two adjacent rectangles. Then, even if moving
///         object should be sliding along the grid of rectangles, it may get stuck at the corner of two adjacent
///         rectangles. This issue is solved by using <see cref="TileColliderComponent" /> for representing tile-based
///         geometry.
///     </para>
///     <para>
///         To create a tile collider, an entity must have both a
///         <see cref="Geisha.Engine.Core.Components.Transform2DComponent" /> and a <see cref="TileColliderComponent" />.
///     </para>
///     <para>
///         Tile colliders are static bodies. Therefore, <see cref="TileColliderComponent" /> does not work together with
///         <see cref="KinematicRigidBody2DComponent" />. They are automatically aligned with the tile grid and cannot be
///         rotated or resized. <see cref="Transform2DComponent.Scale" /> has no effect on the tile colliders. Tile
///         collider center is used as a pivot point for the tile collider. It means that the tile collider is always
///         centered at the specified position in a tile grid. Tile collider must be a root entity.
///     </para>
///     <para>
///         Use <see cref="TileColliderComponent" /> to build static tile-based geometry like a level layout.
///     </para>
/// </remarks>
[ComponentId("Geisha.Engine.Physics.TileColliderComponent")]
public sealed class TileColliderComponent : Collider2DComponent
{
    internal TileColliderComponent(Entity entity) : base(entity)
    {
    }
}

internal sealed class TileColliderComponentFactory : ComponentFactory<TileColliderComponent>
{
    protected override TileColliderComponent CreateComponent(Entity entity) => new(entity);
}