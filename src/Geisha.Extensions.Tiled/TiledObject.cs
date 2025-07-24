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
    }

    public int Id { get; }
    public string Name { get; }
    public string Type { get; }
    public double X { get; }
}