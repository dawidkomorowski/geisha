using System;

namespace Geisha.Engine.Core.Assets;

/// <summary>
///     Represents type safe identifier of an asset.
/// </summary>
public readonly record struct AssetId
{
    /// <summary>
    ///     Creates new, globally unique, instance of <see cref="AssetId" />.
    /// </summary>
    /// <returns>New, globally unique, instance of <see cref="AssetId" />.</returns>
    /// <remarks>Uniqueness of created <see cref="AssetId" /> instances is based on uniqueness of <see cref="Guid" />.</remarks>
    public static AssetId CreateUnique() => new(Guid.NewGuid());

    /// <summary>
    ///     Creates new instance of <see cref="AssetId" /> with <see cref="Value" /> set as specified by
    ///     <paramref name="value" />.
    /// </summary>
    /// <param name="value"></param>
    public AssetId(Guid value)
    {
        Value = value;
    }

    /// <summary>
    ///     Actual value of identifier.
    /// </summary>
    public Guid Value { get; }
}