using System;
using System.Text;

namespace Geisha.Engine.Core.Assets;

/// <summary>
///     Represents a string identifier for an asset type, independent of the actual asset class.
/// </summary>
/// <remarks>
///     A default-initialized instance is invalid. Use <see cref="IsValid" /> to check an instance before accessing
///     <see cref="Value" />.
/// </remarks>
public readonly record struct AssetType
{
    private readonly string? _value;

    /// <summary>
    ///     Creates new instance of <see cref="AssetType" /> given <see cref="string" /> value representing the asset type.
    /// </summary>
    /// <param name="value">
    ///     Value representing the asset type. It can be any kind of unique identifier of the asset type, but it
    ///     may be useful to have it human-readable. An empty string is allowed and is distinct from a default-initialized
    ///     invalid instance.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value" /> is <c>null</c>.</exception>
    public AssetType(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    ///     Gets whether this instance was initialized with a value.
    /// </summary>
    public bool IsValid => _value is not null;

    /// <summary>
    ///     Gets the string identifier represented by this instance.
    /// </summary>
    /// <exception cref="InvalidOperationException">This instance is default-initialized and invalid.</exception>
    public string Value => _value ?? throw new InvalidOperationException("Current instance is invalid.");

    private bool PrintMembers(StringBuilder builder)
    {
        builder.Append($"{nameof(IsValid)} = {IsValid}, ");
        builder.Append($"{nameof(Value)} = {_value}");
        return true;
    }
}