using System;
using System.Collections.Generic;
using System.Xml;

namespace Geisha.Extensions.Tiled;

public sealed class ObjectLayer
{
    private readonly List<TiledObject> _objects = new();

    internal ObjectLayer(XmlElement xml)
    {
        if (xml.Name != "objectgroup")
        {
            throw new ArgumentException($"Expected 'objectgroup' element, but got '{xml.Name}'.");
        }

        Id = xml.GetIntAttribute("id");
        Name = xml.GetStringAttribute("name");

        foreach (XmlElement element in xml.ChildNodes)
        {
            if (element.Name == "object")
            {
                _objects.Add(TiledObject.Create(element));
            }
        }
    }

    public int Id { get; }
    public string Name { get; }
    public IReadOnlyList<TiledObject> Objects => _objects;
}