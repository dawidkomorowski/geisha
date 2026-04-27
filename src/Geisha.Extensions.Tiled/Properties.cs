using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

    public Property this[string name] =>
        _properties.TryGetValue(name, out var property) ? property : throw new KeyNotFoundException($"Property '{name}' not found.");

    public Property? GetPropertyOrNull(string name) => _properties.GetValueOrDefault(name);

    public bool TryGetProperty(string name, [NotNullWhen(true)] out Property? property) => _properties.TryGetValue(name, out property);
}