﻿using System.Globalization;
using System.Xml;

namespace Geisha.Extensions.Tiled;

internal static class XmlElementExtensions
{
    public static bool GetBoolAttribute(this XmlElement element, string attributeName)
    {
        var attribute = element.Attributes[attributeName] ??
                        throw new InvalidTiledMapException($"missing '{attributeName}' attribute in '{element.Name}' element");
        if (!bool.TryParse(attribute.Value, out var value))
        {
            if (!int.TryParse(attribute.Value, out var intValue))
            {
                throw new InvalidTiledMapException($"invalid '{attributeName}' attribute value '{attribute.Value}' in '{element.Name}' element");
            }

            value = intValue != 0;
        }

        return value;
    }

    public static int GetIntAttribute(this XmlElement element, string attributeName)
    {
        var attribute = element.Attributes[attributeName] ??
                        throw new InvalidTiledMapException($"missing '{attributeName}' attribute in '{element.Name}' element");
        if (!int.TryParse(attribute.Value, out var value))
        {
            throw new InvalidTiledMapException($"invalid '{attributeName}' attribute value '{attribute.Value}' in '{element.Name}' element");
        }

        return value;
    }

    public static uint GetUintAttribute(this XmlElement element, string attributeName)
    {
        var attribute = element.Attributes[attributeName] ??
                        throw new InvalidTiledMapException($"missing '{attributeName}' attribute in '{element.Name}' element");
        if (!uint.TryParse(attribute.Value, out var value))
        {
            throw new InvalidTiledMapException($"invalid '{attributeName}' attribute value '{attribute.Value}' in '{element.Name}' element");
        }

        return value;
    }

    public static double GetDoubleAttribute(this XmlElement element, string attributeName, double defaultValue)
    {
        var attribute = element.Attributes[attributeName];
        if (attribute == null)
        {
            return defaultValue;
        }

        if (!double.TryParse(attribute.Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var value ))
        {
            throw new InvalidTiledMapException($"invalid '{attributeName}' attribute value '{attribute.Value}' in '{element.Name}' element");
        }

        return value;
    }

    public static string GetStringAttribute(this XmlElement element, string attributeName)
    {
        var attribute = element.Attributes[attributeName] ??
                        throw new InvalidTiledMapException($"missing '{attributeName}' attribute in '{element.Name}' element");
        return attribute.Value;
    }

    public static string GetStringAttribute(this XmlElement element, string attributeName, string defaultValue)
    {
        var attribute = element.Attributes[attributeName];
        return attribute?.Value ?? defaultValue;
    }
}