using System;
using System.Collections.Generic;
using System.Diagnostics;
using Geisha.Engine.Core.Math;

namespace Geisha.Engine.Physics.PhysicsEngine2D.Internal;

// TODO: TileMap allocates separate list per tile. It is extremely inefficient for big tilemaps as it produces a lot of GC tracked objects.
//       As tiles are static geometry it does not hurt much the steady simulation performance but any modification to tiles (even enabling/disabling)
//       may produce a lot of unnecessary garbage. Better approach would be to keep initially preallocated and reusable list that holds all chunks of tilemap.
internal sealed class TileMap
{
    private readonly SizeD _tileSize;
    private readonly Dictionary<TilePosition, List<RigidBodyId>> _tiles = new();

    public TileMap(SizeD tileSize)
    {
        _tileSize = tileSize;
    }

    public Vector2 AlignPosition(in Vector2 position) => GetTileWorldPosition(GetTilePosition(position));

    public void CreateTile(ref RigidBodyData body)
    {
        Debug.Assert(body.ColliderType is ColliderType.Tile, "Body ColliderType is not Tile.");
        Debug.Assert(!BodyIsPresentInTileMap(ref body), "Body is already in tile map.");

        var tilePosition = GetTilePosition(body.Position);
        if (!_tiles.TryGetValue(tilePosition, out var bodiesInTile))
        {
            bodiesInTile = new List<RigidBodyId>();
            _tiles[tilePosition] = bodiesInTile;
        }

        bodiesInTile.Add(body.Id);

        UpdateTileCluster(tilePosition);
    }

    public Vector2 UpdateTile(ref RigidBodyData body, in Vector2 oldPosition, in Vector2 newPosition)
    {
        Debug.Assert(body.ColliderType is ColliderType.Tile, "Body ColliderType is not Tile.");
        Debug.Assert(body.EnableCollisionDetection, "body.EnableCollisionDetection");
        Debug.Assert(BodyIsPresentInTileMap(ref body), "Body is not in tile map.");

        var oldTilePosition = GetTilePosition(oldPosition);
        var newTilePosition = GetTilePosition(newPosition);

        if (newTilePosition != oldTilePosition)
        {
            if (_tiles.TryGetValue(oldTilePosition, out var bodiesInOldTile))
            {
                bodiesInOldTile.Remove(body.Id);
                if (bodiesInOldTile.Count == 0)
                {
                    _tiles.Remove(oldTilePosition);
                    UpdateTileCluster(oldTilePosition);
                }
            }

            if (!_tiles.TryGetValue(newTilePosition, out var bodiesInNewTile))
            {
                bodiesInNewTile = new List<RigidBodyId>();
                _tiles[newTilePosition] = bodiesInNewTile;
            }

            bodiesInNewTile.Add(body.Id);

            UpdateTileCluster(newTilePosition);
        }


        return GetTileWorldPosition(newTilePosition);
    }

    public void RemoveTile(ref RigidBodyData body)
    {
        Debug.Assert(body.ColliderType is ColliderType.Tile, "Body ColliderType is not Tile.");
        Debug.Assert(BodyIsPresentInTileMap(ref body), "Body is not in tile map.");

        var tilePosition = GetTilePosition(body.Position);
        if (!_tiles.TryGetValue(tilePosition, out var bodiesInTile)) return;

        bodiesInTile.Remove(body.Id);
        if (bodiesInTile.Count != 0) return;

        _tiles.Remove(tilePosition);
        UpdateTileCluster(tilePosition);
    }

    private void UpdateTileCluster(in TilePosition centerTilePosition)
    {
        UpdateCollisionNormalFilter(centerTilePosition);
        UpdateCollisionNormalFilter(centerTilePosition with { X = centerTilePosition.X - 1 });
        UpdateCollisionNormalFilter(centerTilePosition with { X = centerTilePosition.X + 1 });
        UpdateCollisionNormalFilter(centerTilePosition with { Y = centerTilePosition.Y - 1 });
        UpdateCollisionNormalFilter(centerTilePosition with { Y = centerTilePosition.Y + 1 });
    }

    private void UpdateCollisionNormalFilter(in TilePosition tilePosition)
    {
        if (!_tiles.TryGetValue(tilePosition, out var bodiesInThisTile) || bodiesInThisTile.Count == 0)
        {
            return;
        }

        foreach (var bodyId in bodiesInThisTile)
        {
            ref var body = ref Physics2D.Body.GetBodyData(bodyId);
            body.CollisionNormalFilter = CollisionNormalFilter.All;
        }

        if (_tiles.TryGetValue(tilePosition with { X = tilePosition.X - 1 }, out var bodiesInTile) && bodiesInTile.Count > 0)
        {
            foreach (var bodyId in bodiesInThisTile)
            {
                ref var body = ref Physics2D.Body.GetBodyData(bodyId);
                body.CollisionNormalFilter &= ~CollisionNormalFilter.NegativeHorizontal;
            }
        }

        if (_tiles.TryGetValue(tilePosition with { X = tilePosition.X + 1 }, out bodiesInTile) && bodiesInTile.Count > 0)
        {
            foreach (var bodyId in bodiesInThisTile)
            {
                ref var body = ref Physics2D.Body.GetBodyData(bodyId);
                body.CollisionNormalFilter &= ~CollisionNormalFilter.PositiveHorizontal;
            }
        }

        if (_tiles.TryGetValue(tilePosition with { Y = tilePosition.Y - 1 }, out bodiesInTile) && bodiesInTile.Count > 0)
        {
            foreach (var bodyId in bodiesInThisTile)
            {
                ref var body = ref Physics2D.Body.GetBodyData(bodyId);
                body.CollisionNormalFilter &= ~CollisionNormalFilter.NegativeVertical;
            }
        }

        if (_tiles.TryGetValue(tilePosition with { Y = tilePosition.Y + 1 }, out bodiesInTile) && bodiesInTile.Count > 0)
        {
            foreach (var bodyId in bodiesInThisTile)
            {
                ref var body = ref Physics2D.Body.GetBodyData(bodyId);
                body.CollisionNormalFilter &= ~CollisionNormalFilter.PositiveVertical;
            }
        }
    }

    private TilePosition GetTilePosition(in Vector2 position)
    {
        var x = (int)Math.Round(position.X / _tileSize.Width);
        var y = (int)Math.Round(position.Y / _tileSize.Height);
        return new TilePosition(x, y);
    }

    private Vector2 GetTileWorldPosition(in TilePosition tilePosition)
    {
        return new Vector2(tilePosition.X * _tileSize.Width, tilePosition.Y * _tileSize.Height);
    }

    private bool BodyIsPresentInTileMap(ref RigidBodyData body)
    {
        var tilePosition = GetTilePosition(body.Position);
        return _tiles.TryGetValue(tilePosition, out var bodiesInTile) && bodiesInTile.Contains(body.Id);
    }

    private readonly record struct TilePosition(int X, int Y);
}