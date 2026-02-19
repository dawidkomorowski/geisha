using System;

namespace Geisha.Engine.Core.Assets;

/// <summary>
///     Describes an asset entry registered in an <see cref="IAssetStore" />.
/// </summary>
/// <remarks>
///     This type is metadata used by the asset store to identify and locate an asset (id, type and file path).
///     It does not contain the asset data itself.
/// </remarks>
public readonly record struct AssetInfo
{
    private readonly string? _assetFilePath;

    /// <summary>
    ///     Creates new instance of <see cref="AssetInfo" />.
    /// </summary>
    /// <param name="assetId">Unique identifier of the asset.</param>
    /// <param name="assetType">Type of the asset.</param>
    /// <param name="assetFilePath">Absolute path to the asset file.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="assetFilePath" /> is <see langword="null" />.</exception>
    public AssetInfo(AssetId assetId, AssetType assetType, string assetFilePath)
    {
        AssetId = assetId;
        AssetType = assetType;
        _assetFilePath = assetFilePath ?? throw new ArgumentNullException(nameof(assetFilePath));
    }

    /// <summary>
    ///     Unique identifier of the asset.
    /// </summary>
    public AssetId AssetId { get; }

    /// <summary>
    ///     Type of the asset.
    /// </summary>
    public AssetType AssetType { get; }

    /// <summary>
    ///     Absolute path to the asset file.
    /// </summary>
    public string AssetFilePath => _assetFilePath ?? string.Empty;
}