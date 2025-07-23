using System;
using System.Collections.Generic;
using System.Xml;

namespace Geisha.Extensions.Tiled;

public sealed class Properties
{
    private readonly Dictionary<string, Property> _properties = new();

    internal Properties()
    {
    }

    internal Properties(XmlElement xml)
    {
        if (xml.Name != "properties")
        {
            throw new ArgumentException($"Expected 'properties' element, but got '{xml.Name}'.");
        }

        foreach (XmlElement propertyElement in xml.ChildNodes)
        {
            if (propertyElement.Name == "property")
            {
                var property = new Property(propertyElement);
                _properties[property.Name] = property;
            }
        }
    }

    public Property this[string name]
    {
        get
        {
            if (_properties.TryGetValue(name, out var property))
            {
                return property;
            }

            throw new KeyNotFoundException($"Property '{name}' not found.");
        }
    }

    public bool TryGetProperty(string name, out Property? property)
    {
        return _properties.TryGetValue(name, out property);
    }
}