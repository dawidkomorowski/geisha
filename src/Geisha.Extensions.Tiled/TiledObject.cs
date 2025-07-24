using System;
using System.Xml;

namespace Geisha.Extensions.Tiled;

public sealed class TiledObject
{
    public static TiledObject Create(XmlElement xml)
    {
        return new TiledObject(xml);
    }

    private TiledObject(XmlElement xml)
    {
        if (xml.Name != "object")
        {
            throw new ArgumentException($"Expected 'object' element, but got '{xml.Name}'.");
        }

        Id = xml.GetIntAttribute("id");
        Name = xml.GetStringAttribute("name", string.Empty);
        Type = xml.GetStringAttribute("type", string.Empty);
        X = xml.GetDoubleAttribute("x", 0.0);
        Y = xml.GetDoubleAttribute("y", 0.0);
        Width = xml.GetDoubleAttribute("width", 0.0);
        Height = xml.GetDoubleAttribute("height", 0.0);
    }

    public int Id { get; }
    public string Name { get; }
    public string Type { get; }
    public double X { get; }
    public double Y { get; }
    public double Width { get; }
    public double Height { get; }
}