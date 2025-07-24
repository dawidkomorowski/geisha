using System;
using System.Linq;
using System.Xml;

namespace Geisha.Extensions.Tiled;

public abstract class TiledObject
{
    public static TiledObject Create(XmlElement xml)
    {
        if (xml.Name != "object")
        {
            throw new ArgumentException($"Expected 'object' element, but got '{xml.Name}'.");
        }

        if (Point.IsDefined(xml))
        {
            return new Point(xml);
        }

        if (Ellipse.IsDefined(xml))
        {
            return new Ellipse(xml);
        }

        if (Tile.IsDefined(xml))
        {
            return new Tile(xml);
        }

        if (Text.IsDefined(xml))
        {
            return new Text(xml);
        }

        return new Rectangle(xml);
    }

    private TiledObject(XmlElement xml)
    {
        Id = xml.GetIntAttribute("id");
        Name = xml.GetStringAttribute("name", string.Empty);
        Type = xml.GetStringAttribute("type", string.Empty);
        X = xml.GetDoubleAttribute("x", 0.0);
        Y = xml.GetDoubleAttribute("y", 0.0);
        Width = xml.GetDoubleAttribute("width", 0.0);
        Height = xml.GetDoubleAttribute("height", 0.0);

        var propertiesElement = xml.ChildNodes.Cast<XmlElement>().SingleOrDefault(e => e.Name == "properties");
        Properties = propertiesElement is not null ? new Properties(propertiesElement) : new Properties();
    }

    public int Id { get; }
    public string Name { get; }
    public string Type { get; }
    public double X { get; }
    public double Y { get; }
    public double Width { get; }
    public double Height { get; }
    public Properties Properties { get; }

    public sealed class Rectangle : TiledObject
    {
        internal Rectangle(XmlElement xml) : base(xml)
        {
        }
    }

    public sealed class Point : TiledObject
    {
        internal Point(XmlElement xml) : base(xml)
        {
        }

        internal static bool IsDefined(XmlElement xml)
        {
            return xml.ChildNodes.Cast<XmlElement>().Any(element => element.Name == "point");
        }
    }

    public sealed class Ellipse : TiledObject
    {
        internal Ellipse(XmlElement xml) : base(xml)
        {
        }

        internal static bool IsDefined(XmlElement xml)
        {
            return xml.ChildNodes.Cast<XmlElement>().Any(element => element.Name == "ellipse");
        }
    }

    public sealed class Tile : TiledObject
    {
        internal Tile(XmlElement xml) : base(xml)
        {
            GlobalTileId = new GlobalTileId(xml.GetUintAttribute("gid"));
        }

        internal static bool IsDefined(XmlElement xml)
        {
            return xml.Attributes["gid"] is not null;
        }

        public GlobalTileId GlobalTileId { get; }
    }

    public sealed class Text : TiledObject
    {
        internal Text(XmlElement xml) : base(xml)
        {
            Content = xml.ChildNodes.Cast<XmlElement>().Single(element => element.Name == "text").InnerText;
        }

        internal static bool IsDefined(XmlElement xml)
        {
            return xml.ChildNodes.Cast<XmlElement>().Any(element => element.Name == "text");
        }

        public string Content { get; }
    }
}