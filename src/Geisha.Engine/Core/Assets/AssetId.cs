using System;

namespace Geisha.Engine.Core.Assets;

/// <summary>
///     Represents type safe identifier of an asset.
/// </summary>
/// <param name="Value">Actual value of identifier.</param>
public readonly record struct AssetId(Guid Value)
{
    /// <summary>
    ///     Creates new, globally unique, instance of <see cref="AssetId" />.
    /// </summary>
    /// <returns>New, globally unique, instance of <see cref="AssetId" />.</returns>
    /// <remarks>Uniqueness of created <see cref="AssetId" /> instances is based on uniqueness of <see cref="Guid" />.</remarks>
    public static AssetId CreateUnique() => new(Guid.NewGuid());
}