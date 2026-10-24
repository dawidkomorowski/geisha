using System;

namespace Geisha.Engine.Core.Assets;

/// <summary>
///     Represents type information of an asset independent of actual asset class.
/// </summary>
public readonly record struct AssetType
{
    private readonly string? _value;

    /// <summary>
    ///     Creates new instance of <see cref="AssetType" /> given <see cref="string" /> value representing the asset type.
    /// </summary>
    /// <param name="value">
    ///     Value representing the asset type. It can be any kind of unique identifier of the asset type, but it
    ///     may be useful to have it human-readable.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value" /> is <c>null</c>.</exception>
    public AssetType(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    ///     Actual representation of asset type information. Default value is empty <see cref="string" />.
    /// </summary>
    public string Value => _value ?? string.Empty;
}