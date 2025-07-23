using System;
using System.Xml;

namespace Geisha.Extensions.Tiled;

// TODO Add XML documentation comments to public members of this class.
public sealed class TileMap
{
    public static TileMap LoadFromFile(string filePath)
    {
        var xml = new XmlDocument();
        xml.Load(filePath);
        return new TileMap(xml);
    }

    private TileMap(XmlDocument xml)
    {
        var map = xml["map"] ?? throw InvalidFormatException("missing 'map' element");
        var version = map.Attributes["version"] ?? throw InvalidFormatException("missing 'version' attribute in 'map' element");
        Version = version.Value;
        TiledVersion = map.Attributes["tiledversion"]?.Value;
        var orientation = map.Attributes["orientation"] ?? throw InvalidFormatException("missing 'orientation' attribute in 'map' element");
        Orientation = ParseOrientation(orientation.Value);
        var renderOrder = map.Attributes["renderorder"] ?? throw InvalidFormatException("missing 'renderorder' attribute in 'map' element");
        RenderOrder = ParseRenderOrder(renderOrder.Value);
        Width = GetIntAttribute(map, "width");
        Height = GetIntAttribute(map, "height");
        TileWidth = GetIntAttribute(map, "tilewidth");
        TileHeight = GetIntAttribute(map, "tileheight");
        IsInfinite = GetBoolAttribute(map, "infinite");

        if (IsInfinite)
        {
            throw new NotSupportedException($"Infinite maps are not yet supported by {typeof(TileMap).Namespace}.");
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

    private static Orientation ParseOrientation(string orientation)
    {
        return orientation switch
        {
            "orthogonal" => Orientation.Orthogonal,
            "isometric" => Orientation.Isometric,
            "staggered" => Orientation.Staggered,
            "hexagonal" => Orientation.Hexagonal,
            _ => throw InvalidFormatException($"unknown 'orientation' value '{orientation}'")
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
            _ => throw InvalidFormatException($"unknown 'renderorder' value '{renderOrder}'")
        };
    }

    private static bool GetBoolAttribute(XmlElement element, string attributeName)
    {
        var attribute = element.Attributes[attributeName] ?? throw InvalidFormatException($"missing '{attributeName}' attribute in '{element.Name}' element");
        if (!bool.TryParse(attribute.Value, out var value))
        {
            if (!int.TryParse(attribute.Value, out var intValue))
            {
                throw InvalidFormatException($"invalid '{attributeName}' attribute value '{attribute.Value}' in '{element.Name}' element");
            }

            value = intValue != 0;
        }

        return value;
    }

    private static int GetIntAttribute(XmlElement element, string attributeName)
    {
        var attribute = element.Attributes[attributeName] ?? throw InvalidFormatException($"missing '{attributeName}' attribute in '{element.Name}' element");
        if (!int.TryParse(attribute.Value, out var value))
        {
            throw InvalidFormatException($"invalid '{attributeName}' attribute value '{attribute.Value}' in '{element.Name}' element");
        }

        return value;
    }

    private static InvalidOperationException InvalidFormatException(string message)
    {
        return new InvalidOperationException($"Invalid Tiled map XML format: {message}.");
    }
}