using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

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
    [Export(typeof(IAssetLoaderProvider))]
    internal class AssetLoaderProvider : IAssetLoaderProvider
    {
        private readonly IEnumerable<IAssetLoader> _assetLoaders;

        [ImportingConstructor]
        public AssetLoaderProvider([ImportMany] IEnumerable<IAssetLoader> assetLoaders)
        {
            _assetLoaders = assetLoaders;
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