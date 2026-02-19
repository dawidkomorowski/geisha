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

    /// <summary>
    ///     Converts the string representation of an asset identifier to its <see cref="AssetId" /> equivalent.
    /// </summary>
    /// <param name="assetId">A string containing the identifier to convert.</param>
    /// <returns>An <see cref="AssetId" /> equivalent to the identifier contained in <paramref name="assetId" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="assetId" /> is <c>null</c>.</exception>
    /// <exception cref="FormatException"><paramref name="assetId" /> is not in a recognized format.</exception>
    public static AssetId Parse(string assetId) => new(Guid.Parse(assetId));

    /// <summary>
    ///     Converts the string representation of an asset identifier to its <see cref="AssetId" /> equivalent.
    /// </summary>
    /// <param name="assetId">A string containing the identifier to convert.</param>
    /// <param name="result">
    ///     When this method returns, contains the parsed value if the conversion succeeded; otherwise, the default value of
    ///     <see cref="AssetId" />.
    /// </param>
    /// <returns><c>true</c> if <paramref name="assetId" /> was converted successfully; otherwise, <c>false</c>.</returns>
    public static bool TryParse(string? assetId, out AssetId result)
    {
        if (Guid.TryParse(assetId, out var guid))
        {
            result = new AssetId(guid);
            return true;
        }

        result = default;
        return false;
    }
}