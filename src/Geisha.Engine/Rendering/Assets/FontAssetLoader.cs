using System;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Core.FileSystem;
using Geisha.Engine.Rendering.Assets.Serialization;

namespace Geisha.Engine.Rendering.Assets;

internal sealed class FontAssetLoader : IAssetLoader
{
    private readonly IFileSystem _fileSystem;

    public FontAssetLoader(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public AssetType AssetType => RenderingAssetTypes.Font;
    public Type AssetClassType { get; } = typeof(FontFileInfo);

    public object LoadAsset(AssetInfo assetInfo, IAssetStore assetStore)
    {
        using var assetFileStream = _fileSystem.GetFile(assetInfo.AssetFilePath).OpenRead();
        var assetData = AssetData.Load(assetFileStream);
        var fontAssetContent = assetData.ReadJsonContent<FontAssetContent>();

        if (fontAssetContent.FontFilePath is null)
        {
            throw new InvalidOperationException($"{nameof(FontAssetContent)}.{nameof(FontAssetContent.FontFilePath)} cannot be null.");
        }

        var fontFilePath = PathUtils.GetSiblingPath(assetInfo.AssetFilePath, fontAssetContent.FontFilePath);
        return new FontFileInfo(fontFilePath);
    }

    public void UnloadAsset(object asset)
    {
    }
}