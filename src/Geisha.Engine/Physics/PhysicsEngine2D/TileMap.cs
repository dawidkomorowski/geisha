using Geisha.Engine.Core.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Geisha.Engine.Physics.PhysicsEngine2D;

internal sealed class TileMap
{
    private readonly SizeD _tileSize;
    private readonly Dictionary<TilePosition, List<RigidBody2D>> _tiles = new();

    public TileMap(SizeD tileSize)
    {
        _tileSize = tileSize;
    }

    public void CreateTile(RigidBody2D body)
    {
        Debug.Assert(body.ColliderType is ColliderType.Tile, "body.ColliderType is ColliderType.Tile");

        var tilePosition = GetTilePosition(body.Position);
        if (!_tiles.TryGetValue(tilePosition, out var bodiesInTile))
        {
            bodiesInTile = new List<RigidBody2D>();
            _tiles[tilePosition] = bodiesInTile;
        }

        bodiesInTile.Add(body);

        UpdateTileCluster(tilePosition);
    }

    public Vector2 UpdateTile(RigidBody2D body, Vector2 oldPosition, Vector2 newPosition)
    {
        Debug.Assert(body.ColliderType is ColliderType.Tile, "body.ColliderType is ColliderType.Tile");

        var oldTilePosition = GetTilePosition(oldPosition);
        var newTilePosition = GetTilePosition(newPosition);

        if (newTilePosition != oldTilePosition)
        {
            if (_tiles.TryGetValue(oldTilePosition, out var bodiesInOldTile))
            {
                bodiesInOldTile.Remove(body);
                if (bodiesInOldTile.Count == 0)
                {
                    _tiles.Remove(oldTilePosition);
                    UpdateTileCluster(oldTilePosition);
                }
            }

            if (!_tiles.TryGetValue(newTilePosition, out var bodiesInNewTile))
            {
                bodiesInNewTile = new List<RigidBody2D>();
                _tiles[newTilePosition] = bodiesInNewTile;
            }

            bodiesInNewTile.Add(body);

            UpdateTileCluster(newTilePosition);
        }


        return new Vector2(newTilePosition.X * _tileSize.Width, newTilePosition.Y * _tileSize.Height);
    }

    public void RemoveTile(RigidBody2D body)
    {
        Debug.Assert(body.ColliderType is ColliderType.Tile, "body.ColliderType is ColliderType.Tile");

        var tilePosition = GetTilePosition(body.Position);
        if (!_tiles.TryGetValue(tilePosition, out var bodiesInTile)) return;

        bodiesInTile.Remove(body);
        if (bodiesInTile.Count != 0) return;

        _tiles.Remove(tilePosition);
        UpdateTileCluster(tilePosition);
    }

    private void UpdateTileCluster(TilePosition centerTilePosition)
    {
        UpdateCollisionNormalFilter(centerTilePosition);
        UpdateCollisionNormalFilter(centerTilePosition with { X = centerTilePosition.X - 1 });
        UpdateCollisionNormalFilter(centerTilePosition with { X = centerTilePosition.X + 1 });
        UpdateCollisionNormalFilter(centerTilePosition with { Y = centerTilePosition.Y - 1 });
        UpdateCollisionNormalFilter(centerTilePosition with { Y = centerTilePosition.Y + 1 });
    }

    private void UpdateCollisionNormalFilter(TilePosition tilePosition)
    {
        if (!_tiles.TryGetValue(tilePosition, out var bodiesInThisTile) || bodiesInThisTile.Count == 0)
        {
            return;
        }

        foreach (var body in bodiesInThisTile)
        {
            body.CollisionNormalFilter = CollisionNormalFilter.All;
        }

        if (_tiles.TryGetValue(tilePosition with { X = tilePosition.X - 1 }, out var bodiesInTile) && bodiesInTile.Count > 0)
        {
            foreach (var body in bodiesInThisTile)
            {
                body.CollisionNormalFilter &= ~CollisionNormalFilter.NegativeHorizontal;
            }
        }

        if (_tiles.TryGetValue(tilePosition with { X = tilePosition.X + 1 }, out bodiesInTile) && bodiesInTile.Count > 0)
        {
            foreach (var body in bodiesInThisTile)
            {
                body.CollisionNormalFilter &= ~CollisionNormalFilter.PositiveHorizontal;
            }
        }

        if (_tiles.TryGetValue(tilePosition with { Y = tilePosition.Y - 1 }, out bodiesInTile) && bodiesInTile.Count > 0)
        {
            foreach (var body in bodiesInThisTile)
            {
                body.CollisionNormalFilter &= ~CollisionNormalFilter.NegativeVertical;
            }
        }

        if (_tiles.TryGetValue(tilePosition with { Y = tilePosition.Y + 1 }, out bodiesInTile) && bodiesInTile.Count > 0)
        {
            foreach (var body in bodiesInThisTile)
            {
                body.CollisionNormalFilter &= ~CollisionNormalFilter.PositiveVertical;
            }
        }
    }

    private TilePosition GetTilePosition(Vector2 position)
    {
        var x = (int)Math.Round(position.X / _tileSize.Width);
        var y = (int)Math.Round(position.Y / _tileSize.Height);
        return new TilePosition(x, y);
    }

    private readonly record struct TilePosition(int X, int Y);
}