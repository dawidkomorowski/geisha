using System;

namespace Geisha.Engine.Core.Assets;

/// <summary>
///     Represents a type-safe identifier of an asset.
/// </summary>
/// <remarks>
///     <see cref="AssetId" /> is a lightweight wrapper around <see cref="Guid" />.
///     The default value of <see cref="AssetId" /> has <see cref="Value" /> set to <see cref="Guid.Empty" />.
/// </remarks>
/// <param name="Value">Underlying <see cref="Guid" /> value of the identifier.</param>
/// <seealso cref="Guid" />
public readonly record struct AssetId(Guid Value)
{
    /// <summary>
    ///     Creates new instance of <see cref="AssetId" /> with a unique value.
    /// </summary>
    /// <returns>New instance of <see cref="AssetId" /> with a unique value.</returns>
    /// <remarks>
    ///     Uniqueness is based on <see cref="Guid.NewGuid" /> and collisions are extremely unlikely.
    /// </remarks>
    public static AssetId CreateUnique() => new(Guid.NewGuid());
}