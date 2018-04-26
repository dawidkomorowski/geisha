using System;

namespace Geisha.Engine.Core.Assets
{
    /// <inheritdoc />
    /// <summary>
    ///     Adapter class simplifying implementation of <see cref="T:Geisha.Engine.Core.Assets.IAssetLoader" />.
    /// </summary>
    /// <typeparam name="TAsset">Asset type the loader supports.</typeparam>
    public abstract class AssetLoaderAdapter<TAsset> : IAssetLoader
    {
        /// <inheritdoc />
        /// <summary>
        ///     Type of asset supported by loader.
        /// </summary>
        public Type AssetType => typeof(TAsset);

        /// <inheritdoc />
        /// <summary>
        ///     Loads asset from file.
        /// </summary>
        public object Load(string filePath)
        {
            return LoadAsset(filePath);
        }

        /// <summary>
        ///     Should perform actual asset loading.
        /// </summary>
        /// <param name="filePath">Path to asset file.</param>
        /// <returns>Instance of loaded asset.</returns>
        protected abstract TAsset LoadAsset(string filePath);
    }
}