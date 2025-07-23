using System;
using System.Xml;

namespace Geisha.Extensions.Tiled;

public sealed class TileLayer
{
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
    }

    public int Id { get; }
    public string Name { get; }
    public int Width { get; }
    public int Height { get; }
}