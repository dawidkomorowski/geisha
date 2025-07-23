using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Geisha.Extensions.Tiled;

public sealed class TileSet
{
    private readonly Dictionary<uint, Tile> _tiles = new();

    internal TileSet(XmlElement xml)
    {
        if (xml.Name != "tileset")
        {
            throw new ArgumentException($"Expected 'tileset' element, but got '{xml.Name}'.");
        }

        FirstGlobalTileId = new GlobalTileId(xml.GetUintAttribute("firstgid"));
        Source = xml.Attributes["source"]?.Value;

        if (Source is not null)
        {
            xml = ResolveExternalTileSet(xml.OwnerDocument, Source);
        }

        Version = xml.GetStringAttribute("version");
        TiledVersion = xml.Attributes["tiledversion"]?.Value;
        Name = xml.GetStringAttribute("name");
        TileWidth = xml.GetIntAttribute("tilewidth");
        TileHeight = xml.GetIntAttribute("tileheight");
        Spacing = xml.GetIntAttribute("spacing");
        TileCount = xml.GetIntAttribute("tilecount");
        Columns = xml.GetIntAttribute("columns");

        foreach (XmlElement element in xml.ChildNodes)
        {
            if (element.Name == "tile")
            {
                var tile = new Tile(element);
                _tiles[tile.Id] = tile;
            }
        }
    }

    public GlobalTileId FirstGlobalTileId { get; }
    public string? Source { get; }
    public string Version { get; }
    public string? TiledVersion { get; }
    public string Name { get; }
    public int TileWidth { get; }
    public int TileHeight { get; }
    public int Spacing { get; }
    public int TileCount { get; }
    public int Columns { get; }

    internal Tile GetTile(uint localTileId)
    {
        if (_tiles.TryGetValue(localTileId, out var tile))
        {
            return tile;
        }

        return new Tile(localTileId);
    }

    private static XmlElement ResolveExternalTileSet(XmlDocument tileMapDocument, string source)
    {
        var tmxPath = Path.GetDirectoryName(new Uri(tileMapDocument.BaseURI).LocalPath);
        if (tmxPath is null)
        {
            throw new InvalidOperationException("Unable to determine the directory of the TMX file.");
        }

        var tsxPath = Path.GetFullPath(source, tmxPath);
        var tileSetDocument = new XmlDocument();
        tileSetDocument.Load(tsxPath);
        return tileSetDocument["tileset"] ?? throw new InvalidTiledMapException("missing 'tileset' element in external tileset file.");
    }

    internal sealed class Tile
    {
        public Tile(uint id)
        {
            Id = id;
            Properties = new Properties();
        }

        public Tile(XmlElement xml)
        {
            if (xml.Name != "tile")
            {
                throw new ArgumentException($"Expected 'tile' element, but got '{xml.Name}'.");
            }

            Id = xml.GetUintAttribute("id");
            Properties = new Properties();

            foreach (XmlElement element in xml.ChildNodes)
            {
                if (element.Name == "properties")
                {
                    Properties = new Properties(element);
                    break;
                }
            }
        }

        public uint Id { get; }
        public Properties Properties { get; }
    }
}