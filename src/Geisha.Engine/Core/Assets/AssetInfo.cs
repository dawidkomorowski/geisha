using System;

namespace Geisha.Engine.Core.Assets;

/// <summary>
///     Asset registration info to be used in <see cref="IAssetStore" />.
/// </summary>
public readonly record struct AssetInfo
{
    private readonly string? _assetFilePath;

    /// <summary>
    ///     Creates new instance of <see cref="AssetInfo" />.
    /// </summary>
    /// <param name="assetId">Id of asset.</param>
    /// <param name="assetType">Type of asset.</param>
    /// <param name="assetFilePath">Path to asset file.</param>
    public AssetInfo(AssetId assetId, AssetType assetType, string assetFilePath)
    {
        AssetId = assetId;
        AssetType = assetType;
        _assetFilePath = assetFilePath ?? throw new ArgumentNullException(nameof(assetFilePath));
    }

    /// <summary>
    ///     Id of asset.
    /// </summary>
    public AssetId AssetId { get; }

    /// <summary>
    ///     Type of asset.
    /// </summary>
    public AssetType AssetType { get; }

    /// <summary>
    ///     Path to asset file.
    /// </summary>
    public string AssetFilePath => _assetFilePath ?? string.Empty;
}