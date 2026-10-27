using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Assets;
using Geisha.Engine.Rendering.Assets;
using Geisha.Engine.Rendering.Backend;

namespace Geisha.Engine.Rendering;

internal static class CustomFonts
{
    public static void Register(IAssetStore assetStore, IRenderingBackend renderingBackend)
    {
        var fontAssets = assetStore.GetRegisteredAssets().Where(ai => ai.AssetType == RenderingAssetTypes.Font).ToArray();
        var fontPaths = new List<string>();

        foreach (var fontAsset in fontAssets)
        {
            var fontFileInfo = assetStore.GetAsset<FontFileInfo>(fontAsset.AssetId);
            fontPaths.Add(fontFileInfo.Path);
        }

        renderingBackend.RegisterCustomFonts(fontPaths);
    }
}