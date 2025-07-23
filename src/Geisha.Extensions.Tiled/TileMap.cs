using System;
using System.Collections.Generic;
using System.Xml;

namespace Geisha.Extensions.Tiled;

// TODO Add XML documentation comments to public members of this class.
public sealed class TileMap
{
    private readonly List<TileSet> _tileSets = new();
    private readonly List<TileLayer> _tileLayers = new();

    public static TileMap LoadFromFile(string filePath)
    {
        var xml = new XmlDocument();
        xml.Load(filePath);
        return new TileMap(xml);
    }

    private TileMap(XmlDocument xml)
    {
        var map = xml["map"] ?? throw new InvalidTiledMapException("missing 'map' element");
        Version = map.GetStringAttribute("version");
        TiledVersion = map.Attributes["tiledversion"]?.Value;
        Orientation = ParseOrientation(map.GetStringAttribute("orientation"));
        RenderOrder = ParseRenderOrder(map.GetStringAttribute("renderorder"));
        Width = map.GetIntAttribute("width");
        Height = map.GetIntAttribute("height");
        TileWidth = map.GetIntAttribute("tilewidth");
        TileHeight = map.GetIntAttribute("tileheight");
        IsInfinite = map.GetBoolAttribute("infinite");

        if (IsInfinite)
        {
            throw new NotSupportedException($"Infinite maps are not yet supported by {typeof(TileMap).Namespace}.");
        }

        foreach (XmlElement element in map.ChildNodes)
        {
            switch (element.Name)
            {
                case "tileset":
                {
                    var tileSet = new TileSet(element);
                    _tileSets.Add(tileSet);
                    break;
                }
                case "layer":
                {
                    var tileLayer = new TileLayer(this, element);
                    _tileLayers.Add(tileLayer);
                    break;
                }
            }
        }
    }

    public string Version { get; }
    public string? TiledVersion { get; }
    public Orientation Orientation { get; }
    public RenderOrder RenderOrder { get; }
    public int Width { get; }
    public int Height { get; }
    public int TileWidth { get; }
    public int TileHeight { get; }
    public bool IsInfinite { get; }
    public IReadOnlyList<TileSet> TileSets => _tileSets;
    public IReadOnlyList<TileLayer> TileLayers => _tileLayers;

    private static Orientation ParseOrientation(string orientation)
    {
        return orientation switch
        {
            "orthogonal" => Orientation.Orthogonal,
            "isometric" => Orientation.Isometric,
            "staggered" => Orientation.Staggered,
            "hexagonal" => Orientation.Hexagonal,
            _ => throw new InvalidTiledMapException($"unknown 'orientation' value '{orientation}'")
        };
    }

    private static RenderOrder ParseRenderOrder(string renderOrder)
    {
        return renderOrder switch
        {
            "right-down" => RenderOrder.RightDown,
            "right-up" => RenderOrder.RightUp,
            "left-down" => RenderOrder.LeftDown,
            "left-up" => RenderOrder.LeftUp,
            _ => throw new InvalidTiledMapException($"unknown 'renderorder' value '{renderOrder}'")
        };
    }
}