using System;
using System.Collections.Generic;
using System.Linq;

namespace Geisha.Engine.Core.Assets
{
    public interface IAssetLoaderProvider
    {
        IAssetLoader GetLoaderFor(Type assetType);
    }

    internal class AssetLoaderProvider : IAssetLoaderProvider
    {
        private readonly IEnumerable<IAssetLoader> _assetLoaders;

        public AssetLoaderProvider(IEnumerable<IAssetLoader> assetLoaders)
        {
            _assetLoaders = assetLoaders;
        }

        public IAssetLoader GetLoaderFor(Type assetType)
        {
            var loaders = _assetLoaders.Where(l => l.AssetType == assetType).ToList();

            if (loaders.Count == 0)
            {
                throw new GeishaEngineException(
                    $"No loader found for asset type: {assetType}. Single implementation of {nameof(IAssetLoader)} per asset type is expected.");
            }

            if (loaders.Count > 1)
            {
                throw new GeishaEngineException(
                    $"Multiple loaders found for asset type: {assetType}. Single implementation of {nameof(IAssetLoader)} per asset type is expected.");
            }

            return _assetLoaders.Single(l => l.AssetType == assetType);
        }
    }
}