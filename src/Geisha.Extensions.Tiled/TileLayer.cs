using System;
using System.Collections.Generic;
using System.Xml;

namespace Geisha.Extensions.Tiled;

public sealed class TileLayer
{
    private readonly Tile?[][] _tiles;

    internal TileLayer(XmlElement xml)
    {
        if (xml.Name != "layer")
        {
            throw new ArgumentException($"Expected 'layer' element, but got '{xml.Name}'.");
        }

        Id = xml.GetIntAttribute("id");
        Name = xml.GetStringAttribute("name");
        Width = xml.GetIntAttribute("width");
        Height = xml.GetIntAttribute("height");

        _tiles = new Tile?[Width][];
        for (var w = 0; w < Width; w++)
        {
            _tiles[w] = new Tile?[Height];
        }

        var elements = xml.GetElementsByTagName("data");
        if (elements.Count != 1 || elements[0] is not XmlElement dataElement)
        {
            throw new InvalidTiledMapException("missing 'data' element in 'layer'.");
        }

        ParseTiles(dataElement);
    }

    public int Id { get; }
    public string Name { get; }
    public int Width { get; }
    public int Height { get; }
    public IReadOnlyList<IReadOnlyList<Tile?>> Tiles => _tiles;

    private void ParseTiles(XmlElement data)
    {
        var encoding = data.Attributes["encoding"]?.Value;
        if (encoding is not "csv")
        {
            throw new NotSupportedException(
                $"Unsupported encoding '{encoding}' in 'data' element of 'layer'. Only 'csv' encoding is currently supported by {typeof(TileMap).Namespace}.");
        }

        var csv = data.InnerText;
        var rows = csv.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        if (rows.Length != Height)
        {
            throw new InvalidTiledMapException($"Expected {Height} rows in 'data' element, but got {rows.Length}.");
        }

        for (var h = 0; h < Height; h++)
        {
            var row = rows[h].Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (row.Length != Width)
            {
                throw new InvalidTiledMapException($"Expected {Width} columns in row {h} of 'data' element, but got {row.Length}.");
            }

            for (var w = 0; w < Width; w++)
            {
                if (uint.TryParse(row[w], out var globalTileId) && globalTileId > 0)
                {
                    _tiles[w][h] = new Tile(new GlobalTileId(globalTileId));
                }
                else
                {
                    _tiles[w][h] = null;
                }
            }
        }
    }
}