using System;
using System.Collections.Generic;
using System.Linq;
using Geisha.Common.Logging;

namespace Geisha.Engine.Core.Assets
{
    /// <summary>
    ///     Provides functionality to get loader for given asset type.
    /// </summary>
    public interface IAssetLoaderProvider
    {
        /// <summary>
        ///     Returns loader for given asset type.
        /// </summary>
        /// <param name="assetType">Asset type for which loader is requested.</param>
        /// <returns>Loader for given asset type.</returns>
        IAssetLoader GetLoaderFor(Type assetType);
    }

    /// <inheritdoc />
    /// <summary>
    ///     Provides functionality to get loader for given asset type.
    /// </summary>
    internal class AssetLoaderProvider : IAssetLoaderProvider
    {
        private static readonly ILog Log = LogFactory.Create(typeof(AssetLoaderProvider));
        private readonly IEnumerable<IAssetLoader> _assetLoaders;

        public AssetLoaderProvider(IEnumerable<IAssetLoader> assetLoaders)
        {
            _assetLoaders = assetLoaders;

            Log.Info("Discovering asset loaders...");

            foreach (var assetLoader in _assetLoaders)
            {
                Log.Info($"Asset loader found: {assetLoader}");
            }

            Log.Info("Asset loaders discovery completed.");
        }

        /// <inheritdoc />
        /// <summary>
        ///     Returns loader for given asset type.
        /// </summary>
        public IAssetLoader GetLoaderFor(Type assetType)
        {
            var loaders = _assetLoaders.Where(l => l.AssetType == assetType).ToList();

            if (loaders.Count == 0)
            {
                throw new GeishaEngineException(
                    $"No loader found for asset type: {assetType}. Single implementation of {nameof(IAssetLoader)} per asset type is required.");
            }

            if (loaders.Count > 1)
            {
                throw new GeishaEngineException(
                    $"Multiple loaders found for asset type: {assetType}. Single implementation of {nameof(IAssetLoader)} per asset type is required.");
            }

            return _assetLoaders.Single(l => l.AssetType == assetType);
        }
    }
}