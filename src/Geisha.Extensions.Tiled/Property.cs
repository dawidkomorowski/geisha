using System;
using System.Globalization;
using System.Xml;

namespace Geisha.Extensions.Tiled;

public sealed class Property
{
    internal Property(XmlElement xml)
    {
        if (xml.Name != "property")
        {
            throw new ArgumentException($"Expected 'property' element, but got '{xml.Name}'.");
        }

        Name = xml.GetStringAttribute("name");
        Type = ParsePropertyType(xml.GetStringAttribute("type", "string"));
        CustomPropertyType = xml.GetStringAttribute("propertytype", string.Empty);
        Value = xml.GetStringAttribute("value", string.Empty);
    }

    public string Name { get; }
    public PropertyType Type { get; }
    public string CustomPropertyType { get; }
    public string Value { get; }

    public bool BoolValue => bool.Parse(Value);
    public double FloatValue => double.Parse(Value, CultureInfo.InvariantCulture);
    public int IntValue => int.Parse(Value);
    public string StringValue => Value;
    public int ObjectValue => IntValue;

    private static PropertyType ParsePropertyType(string type)
    {
        return type switch
        {
            "string" => PropertyType.String,
            "int" => PropertyType.Int,
            "float" => PropertyType.Float,
            "bool" => PropertyType.Bool,
            "object" => PropertyType.Object,
            _ => throw new NotSupportedException($"Unsupported property type '{type}'.")
        };
    }
}